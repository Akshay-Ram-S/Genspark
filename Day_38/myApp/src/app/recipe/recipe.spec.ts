import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Recipe } from './recipe';
import { RecipeService } from '../services/recipe.service';
import { RecipeModel } from '../models/recipe.model';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('Recipe Component', () => {
  let component: Recipe;
  let fixture: ComponentFixture<Recipe>;

  const mockRecipe: RecipeModel = {
    id: 1,
    name: 'Pav Bhaji',
    cuisine: 'Indian',
    cookTimeMinutes: 30,
    ingredients: 'Potato, Tomato, Butter, Pav',
    image: 'https://example.com/pavbhaji.jpg'
  };

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [Recipe, HttpClientTestingModule ],
      providers: [RecipeService]
    }).compileComponents();

    fixture = TestBed.createComponent(Recipe);
    component = fixture.componentInstance;
    component.recipe = mockRecipe;

    fixture.detectChanges(); 
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('Input received', () => {
    expect(component.recipe).toEqual(mockRecipe);
  });


  
});
