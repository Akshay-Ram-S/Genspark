/*
Write a program that:

Has a predefined secret word (e.g., "GAME").
Accepts user input as a 4-letter word guess.
Compares the guess to the secret word and outputs:
X Bulls: number of letters in the correct position.
Y Cows: number of correct letters in the wrong position.
Continues until the user gets 4 Bulls (i.e., correct guess).

*/

namespace Tasks
{
    internal class Program_9
    {

        static (int, int) CompareGuess(string? guess, String secret)
        {
            int bulls = 0, cows = 0;
            bool[] secretArr = new bool[4];
            bool[] guessArr = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                if (secret[i] == guess[i])
                {
                    bulls++;
                    secretArr[i] = true;
                    guessArr[i] = true;
                }
            }

            for (int i = 0; i < 4; i++)
            {
                if (!guessArr[i])
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (!secretArr[j] && guess[i] == secret[j])
                        {
                            cows++;
                            secretArr[j] = true;
                            guessArr[i] = true;
                            break;
                        }
                    }
                }
            }

            return (bulls, cows);
        }
        static void StartGuess()
        {
            int attempts = 0;
            string secret = "GAME";
            string? guess;

            while (true)
            {
                Console.Write("Please guess the secret word: ");
                guess = Console.ReadLine();
                while (guess.Length != 4)
                {
                    Console.Write("Invalid guess. Please enter a 4-letter word: ");
                    guess = Console.ReadLine();
                }

                (int, int) res = CompareGuess(guess, secret);
                attempts++;

                if (res.Item1 == 4)
                {
                    Console.WriteLine($"Congrats! Correct Guess. \nAttempts: {attempts}");
                    break;
                }

                Console.WriteLine($"Bulls: {res.Item1}, Cows: {res.Item2}");

            }


        }
        public void Run()
        {
            StartGuess();
        }
    }
}