import { Component, inject, signal } from '@angular/core';
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
  recipes = signal<RecipeModel[] | null>(null);

  constructor(public recipeService: RecipeService) {
    this.loadRecipes();
  }

  loadRecipes() {
    this.recipeService.getAllRecipes().subscribe({
      next: (data: any) => {
        console.log('Recipes loaded:', data);
        this.recipes.set(data.recipes ?? []);
      },
      error: () => {
        this.recipes.set([]);
      }
    });
  }
}
