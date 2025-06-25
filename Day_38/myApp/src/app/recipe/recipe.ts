import { Component, inject, Input } from '@angular/core';
import { RecipeModel } from '../models/recipe.model';
import { RecipeService } from '../services/recipe.service';

@Component({
  selector: 'app-recipe',
  standalone: true,
  templateUrl: './recipe.html',
  styleUrls: ['./recipe.css']
})

export class Recipe {
@Input() recipe:RecipeModel|null = new RecipeModel();
private productService = inject(RecipeService);
}
