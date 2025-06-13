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
}
