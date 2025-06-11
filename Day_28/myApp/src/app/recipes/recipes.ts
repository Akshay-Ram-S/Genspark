import { Component, inject, effect, signal } from '@angular/core';
import { Recipe } from '../recipe/recipe';
import { RecipeService } from '../services/recipe.service';
import { RecipeModel } from '../models/recipe.model';

@Component({
  selector: 'app-recipes',
  standalone: true,
  imports: [Recipe],
  templateUrl: './recipes.html',
  styleUrls: ['./recipes.css']
})
export class Recipes {
  private recipeService = inject(RecipeService);

  recipes = signal<RecipeModel[] | null>(null);

  constructor() {
    this.loadRecipes();
  }

  loadRecipes() {
    this.recipeService.getAllRecipes().subscribe({
      next: (data: any) => {
        this.recipes.set([]);
      },
      error: () => {
        this.recipes.set([]);
      }
    });
  }
}
