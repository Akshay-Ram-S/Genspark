import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { RecipeService } from './recipe.service';


describe('RecipeService', () => {
  let service: RecipeService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [RecipeService]
    });
    service = TestBed.inject(RecipeService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('Get recipes from API', () => {
    const mockRecipes = [
      {
        id: 1,
        title: 'Spaghetti Carbonara',
        description: 'A classic Italian pasta dish.',
        ingredients: ['spaghetti', 'eggs', 'parmesan cheese', 'black pepper'],
        instructions: 'Cook spaghetti. Mix eggs and cheese. Combine all.',
        imageUrl: 'https://example.com/spaghetti.jpg'
      },
      {
        id: 2,
        title: 'Chicken Curry',
        description: 'A spicy and flavorful chicken dish.',
        ingredients: ['chicken', 'curry powder', 'coconut milk', 'rice'],
        instructions: 'Cook chicken. Add spices and coconut milk. Serve with rice.',
        imageUrl: 'https://example.com/chicken-curry.jpg'
      }
    ];

    service.getAllRecipes().subscribe(response => {
        expect(response.recipes).toEqual(mockRecipes); 
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');
    req.flush({ recipes: mockRecipes });
  });


  it('HTTP Error response', () => {
    service.getAllRecipes().subscribe({
      next: () => fail('Simulate Error'),
      error: (error) => {
        expect(error.status).toBe(500);
        expect(error.error).toBe('Internal Server Error');
      }
    });

    const req = httpMock.expectOne('https://dummyjson.com/recipes');
    expect(req.request.method).toBe('GET');
    req.flush('Internal Server Error', { status: 500, statusText: 'Internal Server Error' });
  });

});

           