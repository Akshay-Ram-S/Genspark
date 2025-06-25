import { ComponentFixture, TestBed } from '@angular/core/testing';
import { Recipes } from './recipes';
import { Recipe } from '../recipe/recipe';
import { RecipeService } from '../services/recipe.service';
import { of, throwError } from 'rxjs';

describe('Recipes Component', () => {
  let fixture: ComponentFixture<Recipes>;
  let component: Recipes;
  let recipeServiceSpy: jasmine.SpyObj<RecipeService>;

  const mockRecipes = [
    {
      id: 1,
      name: 'Spaghetti Carbonara',
      cuisine: 'Italian',
      cookTimeMinutes: 20,
      ingredients: 'spaghetti, eggs, cheese',
      image: 'https://example.com/image.jpg'
    }
  ];

  beforeEach(async () => {
    recipeServiceSpy = jasmine.createSpyObj('RecipeService', ['getAllRecipes']);
    recipeServiceSpy.getAllRecipes.and.returnValue(of({ recipes: mockRecipes }));

    await TestBed.configureTestingModule({
      imports: [Recipes, Recipe], 
      providers: [
        { provide: RecipeService, useValue: recipeServiceSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(Recipes);
    component = fixture.componentInstance;
    fixture.detectChanges(); 
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('load recipes successfully', () => {
    expect(component.recipes()).toEqual(mockRecipes);
    expect(recipeServiceSpy.getAllRecipes).toHaveBeenCalledTimes(1);
  });

  it('set empty recipes on errors', () => {
    recipeServiceSpy.getAllRecipes.and.returnValue(throwError(() => new Error('Server error')));
    component.loadRecipes(); 
    expect(component.recipes()).toEqual([]);
  });

});
