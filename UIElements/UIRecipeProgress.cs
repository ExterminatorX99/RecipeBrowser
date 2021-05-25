﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection;
using Terraria;
using Terraria.Graphics;
using Terraria.ModLoader;
using Terraria.UI;

namespace RecipeBrowser.UIElements
{
	internal class UIRecipeProgress : UIElement
	{
		private int order;
		private int owner; // which player are we tracking the progress for.
		Texture2D playerBackGroundTexture;

		static MethodInfo drawPlayerHeadMethodInfo;

		public UIRecipeProgress(int index, Recipe recipe, int order, int owner)
		{
			if (drawPlayerHeadMethodInfo == null)
			{
				drawPlayerHeadMethodInfo = typeof(Main).GetMethod("DrawPlayerHead", BindingFlags.Instance | BindingFlags.NonPublic);
			}

			playerBackGroundTexture = TextureManager.Load("Images/UI/PlayerBackground");
			this.order = order;
			this.owner = owner;
			// TODO: Implement Craft Path for teammates.
			UIMockRecipeSlot create = new UIMockRecipeSlot(RecipeCatalogueUI.instance.recipeSlots[index], owner != Main.myPlayer ? .5f : 0.75f);
			create.Recalculate();
			create.Left.Set(-create.Width.Pixels - (owner != Main.myPlayer ? 23 : 0), 1f);
			var b = create.GetOuterDimensions();
			Append(create);
			int x = (owner != Main.myPlayer ? 23 : 0);
			x += (int)b.Width + 2;
			for (int j = 0; j < recipe.requiredItem.Length; j++)
			{
				if (recipe.requiredItem[j].type > 0)
				{
					Item item = new Item();
					item.SetDefaults(recipe.requiredItem[j].type);
					UITrackIngredientSlot ingredient = new UITrackIngredientSlot(item, recipe.requiredItem[j].stack, recipe, j, owner, owner != Main.myPlayer ? .5f : 0.75f);
					x += (int)b.Width + 2;
					ingredient.Left.Set(-x, 1f);

					RecipeCatalogueUI.OverrideForGroups(recipe, ingredient.item);

					Append(ingredient);
				}
			}
			Height.Pixels = b.Height;
			Width.Pixels = x + 12;
		}

		private bool updateNeeded;

		public override void Update(GameTime gameTime)
		{
			if (!updateNeeded) return;
			updateNeeded = false;
		}

		public override int CompareTo(object obj)
		{
			UIRecipeProgress other = obj as UIRecipeProgress;
			return order.CompareTo(other.order);
		}

		protected override void DrawSelf(SpriteBatch spriteBatch)
		{
			base.DrawSelf(spriteBatch);
			if (IsMouseHovering && owner != Main.myPlayer)
			{
				Main.hoverItemName = Main.player[owner].name; //+ "'s Recipe";
				var a = GetInnerDimensions().ToRectangle();
				// protected void DrawPlayerHead(Player drawPlayer, float X, float Y, float Alpha = 1f, float Scale = 1f)
				drawPlayerHeadMethodInfo.Invoke(Main.instance, new object[] { Main.player[owner], a.Right - 16, a.Y + 8, 1f, 1f });
			}
		}
	}
}