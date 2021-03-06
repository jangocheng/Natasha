﻿using Natasha;
using System;

namespace OperatorTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TestOperator();
            TestWhile();
            TestIf();
            Console.ReadKey();
        }

        public static void TestOperator()
        {
            Delegate showResult = EHandler.CreateMethod<ENull>((il) =>
            {
                EMethod method = typeof(Console);
                EVar emit_A = EVar.CreateWithoutTempVar(12);
                EVar emit_B = EVar.CreateVarFromObject(13);
                method.ExecuteMethod<int>("WriteLine", emit_A + emit_B);
                method.ExecuteMethod<int>("WriteLine", emit_A + 1);
                method.ExecuteMethod<int>("WriteLine", 1 + 1);
                method.ExecuteMethod<int>("WriteLine", emit_B++);

            }).Compile();

            ((Action)showResult)();

            TestClass t = new TestClass();
            t.Field = 10;
            t.Property = 20;

            TestStruct t2 = new TestStruct();
            t2.Field = 90;
            t2.Property = 80;
            t.Next = t2;
            showResult = EHandler.CreateMethod<ENull>((il) =>
            {
                EMethod method = typeof(Console);
                EVar emit_A = EVar.CreateWithoutTempVar(12);
                EVar emit_B = EVar.CreateVarFromObject(13);
                EModel model = EModel.CreateModelFromObject(t);

                method.ExecuteMethod<int>("WriteLine", model.DLoadValue("Field").DelayAction);
                method.ExecuteMethod<int>("WriteLine", (model.DLoadValue("Field").Operator++).DelayAction);
                method.ExecuteMethod<int>("WriteLine", model.DLoadValue("Field") + emit_A);
                method.ExecuteMethod<int>("WriteLine", model.DLoadValue("Field").Operator + 10);
                method.ExecuteMethod<int>("WriteLine", (model.DLoad("Next").DLoadValue("Property").Operator++).DelayAction);
                method.ExecuteMethod<int>("WriteLine", (model.DLoad("Next").DLoadValue("Property").Operator + 10));
                method.ExecuteMethod<int>("WriteLine", emit_B + model.DLoadValue("Property").Operator);
                method.ExecuteMethod<int>("WriteLine", emit_B++);

            }).Compile();
            ((Action)showResult)();
        }

        public static void TestWhile()
        {

            Delegate showResult = EHandler.CreateMethod<ENull>((il) =>
            {
                EMethod method = typeof(Console);

                EVar emit_A = EVar.CreateWithoutTempVar(16);
                EVar emit_B = EVar.CreateVarFromObject(20);
                ELoop.While(emit_A < emit_B)(() =>
                {
                    method.ExecuteMethod<int>("WriteLine", emit_B);
                    emit_B--;
                });


                TestClass t = new TestClass() { Field = 10 };
                EModel model = EModel.CreateModelFromObject(t);
                ELoop.While(model.DLoadValue("Field").Operator < emit_B)(() =>
                {
                    //这里需要传递委托，不传递委托则返回的是model类型而不是int类型
                    method.ExecuteMethod<int>("WriteLine", model.DLoadValue("Field").DelayAction);
                    model.DLoadValue("Field").Operator++;
                });

                ELoop.While(model.DLoadValue("Field").Operator != 25)(() =>
                {
                    method.ExecuteMethod<int>("WriteLine", model.DLoadValue("Field").DelayAction);
                    model.DLoadValue("Field").Operator++;
                });

            }).Compile();

            ((Action)showResult)();
        }

        public static void TestIf()
        {
            Delegate showResult = EHandler.CreateMethod<ENull>((il) =>
            {
                EMethod method = typeof(Console);

                EVar emit_A = EVar.CreateWithoutTempVar(10);
                EVar emit_B = EVar.CreateVarFromObject(20);

                TestClass t = new TestClass() { Field = 10 };
                t.PropertyName = "3";
                EModel model = EModel.CreateModelFromObject(t);

                EJudge.If(emit_A == model.DLoadValue("Field").Operator)(() =>
                {
                    method.ExecuteMethod<string>("WriteLine", "相等");

                }).ElseIf(emit_A > emit_B)(() =>
                {
                    method.ExecuteMethod<int>("WriteLine", emit_A);

                }).Else(() =>
                {
                    method.ExecuteMethod<int>("WriteLine", emit_B);

                });



                EVar string_A = "6";
                EVar string_B = "2";

                EJudge.If(string_A == "1")(() =>
                {
                    method.ExecuteMethod<string>("WriteLine", string_A);

                }).ElseIf(string_A == model.DLoadValue("PropertyName").Operator)(() =>
                {
                    method.ExecuteMethod<string>("WriteLine", string_A);

                }).Else(() =>
                {
                    method.ExecuteMethod<string>("WriteLine", string_B);
                });

            }).Compile();

            ((Action)showResult)();
        }
    }
}
