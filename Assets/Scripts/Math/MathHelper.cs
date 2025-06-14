using UnityEngine;

public static class MathHelper
{
	public static int GetFactorial(int Num)
	{
		int Result = 1;

		for (int i = 2; i <= Num; ++i)
		{
			Result *= i;
		}
		
		return Result;
	}
	
	public static int GetCombination(int N, int R)
	{
		int Factorial_N = GetFactorial(N);
		int Factorial_R = GetFactorial(R);

		int Factorial_NR = GetFactorial(N - R);
		
		return Factorial_N / (Factorial_R * Factorial_NR);
	}
	public static bool DoTwoLinesIntersect(Vector2 Point_A1, Vector2 Point_A2, Vector2 Point_B1, Vector2 Point_B2)
	{
		float Denimator = (Point_A1.x - Point_A2.x) * (Point_B1.y - Point_B2.y) -
		                  (Point_A1.y - Point_A2.y) * (Point_B1.x - Point_B2.x);

		if (Mathf.Approximately(Denimator, 0.0f))
		{
			return false;
		}

		float t = ((Point_A1.x - Point_B1.x) * (Point_B1.y - Point_B2.y) - 
		           (Point_A1.y - Point_B1.y) * (Point_B1.x - Point_B2.x)) / Denimator;
    
		float u = -((Point_A1.x - Point_A2.x) * (Point_A1.y - Point_B1.y) - 
		            (Point_A1.y - Point_A2.y) * (Point_A1.x - Point_B1.x)) / Denimator;
		
		return (t >= 0 && t <= 1) && (u >= 0 && u <= 1);
	}
}
