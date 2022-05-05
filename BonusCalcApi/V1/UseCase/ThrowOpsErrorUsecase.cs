namespace BonusCalcApi.V1.UseCase
{
    public static class ThrowOpsErrorUsecase
    {
        public static void Execute()
        {
            throw new TestOpsErrorException("This is a test exception to test our integrations");
        }
    }
}
