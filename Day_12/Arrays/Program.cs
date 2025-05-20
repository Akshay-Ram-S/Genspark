
/*
1) Design a C# console app that uses a jagged array to store data for Instagram posts by multiple users. Each user can have a different number of posts, 
and each post stores a caption and number of likes.

You have N users, and each user can have M posts (varies per user).
Each post has:
- A caption (string)
- A number of likes (int)

Store this in a jagged array, where each index represents one user's list of posts.
Display all posts grouped by user. 
*/

namespace tasks
{
    public class Post
    {
        public string? caption { get; set; }
        public int likes { get; set; }
        public Post(string _caption, int _likes)
        {
            caption = _caption;
            likes = _likes;
        }

    }
    internal class Program
    {
        static void GetUserPostData(int users, out object[][] userPosts)
        {
            userPosts = new object[users][];

            for (int i = 0; i < users; i++)
            {
                Console.Write($"\nUser {i + 1}: How many posts? ");
                int posts;
                while (!int.TryParse(Console.ReadLine(), out posts) && posts >= 0)
                {
                    Console.Write("Invalid Input. Try again: ");
                }

                userPosts[i] = new object[posts];

                for (int j = 0; j < posts; j++)
                {
                    Console.Write($"Enter caption for Post {j + 1}: ");
                    string caption = Console.ReadLine();

                    Console.Write("Enter likes: ");
                    int likes;
                    while (!int.TryParse(Console.ReadLine(), out likes))
                    {
                        Console.Write("Invalid Input. Try again: ");
                    }

                    userPosts[i][j] = new Post(caption, likes);
                }
            }
        }

        static void DisplayPosts(int users, object[][] userPosts)
        {
            Console.WriteLine();
            for (int i = 0; i < users; i++)
            {
                Console.WriteLine($"User {i + 1}:");

                for (int j = 0; j < userPosts[i].Length; j++)
                {
                    Post post = (Post)userPosts[i][j];
                    Console.WriteLine($"Post {j + 1} - Caption: {post.caption} | Likes: {post.likes}\n");
                }

            }
        }

        static void GetTotalUsers()
        {
            Console.Write("Enter number of users: ");
            int users;
            while (!int.TryParse(Console.ReadLine(), out users) && users >= 0 )
            {
                Console.WriteLine("Invalid input. Try again.");
            }

            object[][] userPosts;
            GetUserPostData(users, out userPosts);
            DisplayPosts(users, userPosts);
        }

        public static void Main()
        {
            GetTotalUsers();
        }

    }

}


