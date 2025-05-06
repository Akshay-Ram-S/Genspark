using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using System.Text;
using System;

class Result
{

    /*
     * Complete the 'plusMinus' function below.
     *
     * The function accepts INTEGER_ARRAY arr as parameter.
     */

    public static void plusMinus(List<int> arr)
    {
        int total_positive = 0, total_negative = 0, total_zero = 0;
        foreach(int num in arr){
            if(num > 0){
                total_positive++;
            }
            else if(num < 0){
                total_negative++;
            }
            else{
                total_zero++;
            }
        }
        int size = arr.Count;
        double positive, negative, zero;
        positive = (double) total_positive / size;
        negative = (double) total_negative / size;
        zero = (double) total_zero / size; 
        Console.WriteLine(Math.Round(positive, 6));
        Console.WriteLine(Math.Round(negative, 6));
        Console.WriteLine(Math.Round(zero, 6));
    }

}

class Solution
{
    public static void Main(string[] args)
    {
        int n = Convert.ToInt32(Console.ReadLine().Trim());

        List<int> arr = Console.ReadLine().TrimEnd().Split(' ').ToList().Select(arrTemp => Convert.ToInt32(arrTemp)).ToList();

        Result.plusMinus(arr);
    }
}
