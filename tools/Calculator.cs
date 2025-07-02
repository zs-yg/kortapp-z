using System;

namespace AppStore
{
    /// <summary>
    /// 提供基本数学运算功能的工具类
    /// </summary>
    public static class Calculator
    {
        /// <summary>
        /// 加法运算
        /// </summary>
        public static double Add(double a, double b) => a + b;

        /// <summary>
        /// 减法运算
        /// </summary>
        public static double Subtract(double a, double b) => a - b;

        /// <summary>
        /// 乘法运算
        /// </summary>
        public static double Multiply(double a, double b) => a * b;

        /// <summary>
        /// 除法运算
        /// </summary>
        public static double Divide(double a, double b)
        {
            if (b == 0) throw new DivideByZeroException("除数不能为零");
            return a / b;
        }
    }
}
