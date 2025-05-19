// Valid Sudoku or not

namespace Tasks
{
    internal class Program_11
    {

        static bool IsValid(int[,] mat)
        {
            int[,] rows = new int[9, 9];
            int[,] cols = new int[9, 9];
            int[,] subMat = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (mat[i, j] == 0)
                        continue;

                    int val = mat[i, j] - 1;

                    if (rows[i, val] == 1)
                        return false;
                    rows[i, val] = 1;

                    if (cols[j, val] == 1)
                        return false;
                    cols[j, val] = 1;

                    int idx = (i / 3) * 3 + j / 3;
                    if (subMat[idx, val] == 1)
                        return false;
                    subMat[idx, val] = 1;
                }
            }

            return true;
        }

        public void Run()
        {
            int[,] mat = {
                {7, 9, 2, 1, 5, 4, 3, 8, 6},
                {6, 4, 3, 8, 2, 7, 1, 5, 9},
                {8, 5, 1, 3, 9, 6, 7, 2, 4},
                {2, 6, 5, 9, 7, 3, 8, 4, 1},
                {4, 8, 9, 5, 6, 1, 2, 7, 3},
                {3, 1, 7, 4, 8, 2, 9, 6, 5},
                {1, 3, 6, 7, 4, 8, 5, 9, 2},
                {9, 7, 4, 2, 1, 5, 6, 3, 8},
                {5, 2, 8, 6, 3, 9, 4, 1, 7}
            };

            Console.WriteLine(IsValid(mat) ? "Sudoku Board is Valid" : "Sudoku Board is Not Valid");
        }
    }
}

