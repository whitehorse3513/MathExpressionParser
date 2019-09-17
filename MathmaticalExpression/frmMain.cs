using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MathmaticalExpression
{
    public partial class frmMain : Form
    {
        public string ExprString;
        public int LexPos;
        public double LexicalAttribute;
        public string LexicalAttribute2;
        public string PostFixString;
        public List<string> PostFixQueue;
        public int Given;

        public const int NONE = 0;
        public const int LargestSize = 54;

        public string LookAhead;

        static Dictionary<string, int> id_dict = new Dictionary<string, int>();
        static int id_count = 0;

        public frmMain()
        {
            InitializeComponent();
        }

        private void frmMain_Load(object sender, EventArgs e)
        {

        }

        private void SetVariables()
        {
            Given = 0;
            LexPos = 0;
            ExprString = txtExpression.Text.ToUpper();
            PostFixString = "";
            PostFixQueue = new List<string>();
        }

        private string LexicalAnalyzer()
        {
            string Temp = "";
            while (LexPos < ExprString.Length)
            {
                LexicalAttribute = 0;
                LexicalAttribute2 = "";
                LexPos++;

                switch (ExprString[LexPos - 1])
                {
                    case ' ':
                    case (char)9:
                        break;
                    case '\0':
                    case '+':
                    case '-':
                    case '*':
                    case '/':
                    case '\\':
                    case ':':
                    case '(':
                    case ')':
                    case ',':
                        return ExprString[LexPos - 1].ToString();
                    case (char)13:
                        if (ExprString[LexPos] != 10)
                            SyntaxError();
                        else
                            LexPos++;
                        break;
                    case (char)10:
                        SyntaxError();
                        break;
                    default:
                        if (IsAlpha(ExprString[LexPos - 1].ToString()))
                        {

                            while (IsAlphaNumberic(ExprString[LexPos - 1].ToString()))
                            {
                                Temp = Temp + ExprString[LexPos - 1].ToString();
                                LexPos++;
                                if (LexPos > ExprString.Length)
                                    break;
                            }
                            LexPos--;
                            if (Temp.Length > LargestSize)
                                SyntaxError();
                            if (Temp.ToUpper() == "MOD")
                            {
                                return "MOD";
                            }
                            else if (LookUpFunctionNumber(Temp) != 0)
                            {
                                LexicalAttribute = LookUpFunctionNumber(Temp);
                                LexicalAttribute2 = Temp.ToUpper();
                                return "FUNC";
                            }
                            else
                            {
                                LexicalAttribute = GetID(Temp);
                                return "ID";
                            }
                        }
                        else if (IsNumber(ExprString[LexPos - 1].ToString()))
                        {
                            while (IsNumber(ExprString[LexPos - 1].ToString()) || ExprString[LexPos - 1].ToString() == ".")
                            {
                                Temp = Temp + ExprString[LexPos - 1].ToString();
                                LexPos++;
                                if (LexPos > ExprString.Length)
                                    break;
                            }
                            LexicalAttribute = Convert.ToDouble(Temp);
                            LexPos = LexPos - 1;
                            return "NUM";
                        }
                        break;
                }
            }
            return "";
        }

        private int GetID(string IDName)
        {
            if (id_dict.ContainsKey(IDName))
            {
                return id_dict[IDName];
            }
            else
            {
                id_dict.Add(IDName, id_count++);
                return id_dict[IDName];
            }
        }

        private int LookUpFunctionNumber(string FunctionName)
        {
            int FunctionNumber = 0;
            switch(FunctionName)
            {
                case "SQR":
                    FunctionNumber = 1;
                    break;
                case "SQUARE":
                    FunctionNumber = 2;
                    break;
                case "CUBE":
                    FunctionNumber = 3;
                    break;
                case "INT":
                    FunctionNumber = 4;
                    break;
                case "FIX":
                    FunctionNumber = 5;
                    break;
                case "ROUND":
                    FunctionNumber = 6;
                    break;
                case "RND":
                    FunctionNumber = 7;
                    break;
                case "ABS":
                    FunctionNumber = 8;
                    break;
                case "SGN":
                    FunctionNumber = 9;
                    break;
                case "REPROC":
                    FunctionNumber = 10;
                    break;
                case "INVSGN":
                    FunctionNumber = 11;
                    break;
                case "FACT":
                    FunctionNumber = 12;
                    break;
                case "SIN":
                    FunctionNumber = 13;
                    break;
                case "COS":
                    FunctionNumber = 14;
                    break;
                case "TAN":
                    FunctionNumber = 15;
                    break;
                case "SEC":
                    FunctionNumber = 16;
                    break;
                case "COSEC":
                    FunctionNumber = 17;
                    break;
                case "COT":
                    FunctionNumber = 18;
                    break;
                case "HSIN":
                    FunctionNumber = 19;
                    break;
                case "HCOS":
                    FunctionNumber = 20;
                    break;
                case "HTAN":
                    FunctionNumber = 21;
                    break;
                case "HSEC":
                    FunctionNumber = 22;
                    break;
                case "HCOSEC":
                    FunctionNumber = 23;
                    break;
                case "HCOT":
                    FunctionNumber = 24;
                    break;
            }
            return FunctionNumber;
        }

        private void MatchAndIncrement(string CheckValue)
        {
            if (LookAhead == CheckValue)
                LookAhead = LexicalAnalyzer();
            else
                SyntaxError();
        }

        private void ParseAll()
        {
            LookAhead = LexicalAnalyzer();

            while(LookAhead != "")
            {
                ParseExpression();
            }
        }

        private void ParseExpression()
        {
            string Temp;
            ParseTerm();

            while(true)
            {
                switch(LookAhead)
                {
                    case "+":
                    case "-":
                        Temp = LookAhead;
                        MatchAndIncrement(LookAhead);
                        ParseTerm();
                        AddToPostFix(Temp, NONE);
                        break;
                    default:
                        return;
                }
            }
        }

        private void ParseFactor()
        {
            string Temp;
            switch(LookAhead)
            {
                case "(":
                    MatchAndIncrement("(");
                    ParseExpression();
                    MatchAndIncrement(")");
                    break;
                case "NUM":
                case "ID":
                    AddToPostFix(LookAhead, LexicalAttribute);
                    MatchAndIncrement(LookAhead);
                    break;
                case "FUNC":
                    Temp = LexicalAttribute2;
                    MatchAndIncrement("FUNC");
                    MatchAndIncrement("(");
                    ParseListOfExpressions();
                    MatchAndIncrement(")");
                    AddToPostFix(Temp, NONE);
                    break;
                default:
                    SyntaxError();
                    break;
            }
        }

        private void ParseListOfExpressions()
        {
            ParseExpression();
            while(true)
            {
                switch(LookAhead)
                {
                    case ",":
                        MatchAndIncrement(LookAhead);
                        ParseExpression();
                        break;
                    default:
                        return;
                }
            }
        }

        private void ParseTerm()
        {
            string Temp;
            ParseFactor();
            while(true)
            {
                switch(LookAhead)
                {
                    case "*":
                    case "\\":
                    case "/":
                    case "MOD":
                        Temp = LookAhead;
                        MatchAndIncrement(LookAhead);
                        ParseFactor();
                        AddToPostFix(Temp, NONE);
                        break;
                    default:
                        return;
                }
            }
        }

        private void AddToPostFix(string TokenVal, double TokenAttr)
        {
            switch(TokenVal)
            {
                case "+":
                case "-":
                case "/":
                case "\\":
                case "*":
                case "MOD":
                    PostFixString = PostFixString + TokenVal;
                    PostFixQueue.Add(TokenVal);
                    break;
                case "NUM":
                case "ID":
                    PostFixString = PostFixString + TokenAttr.ToString() + " ";
                    PostFixQueue.Add(TokenAttr.ToString());
                    break;
                default:
                    PostFixString = PostFixString + TokenVal + " ";
                    PostFixQueue.Add(TokenVal);
                    break;
            }
        }

        private void SyntaxError()
        {
            if (Given == 0)
            {
                MessageBox.Show("Syntax Error", "Error");
                Given = -1;
            }
            //Error 1
        }

        private bool IsAlpha(string TestNo)
        {
            if (TestNo.Length == 0)
                return false;
            char cTestNo = TestNo[0];
            if((cTestNo >= 65 && cTestNo <= 90) || (cTestNo >= 97 && cTestNo <= 122))
                return true;
            else 
                return false;
        }

        private bool IsAlphaNumberic(string TestNo)
        {
            if (TestNo.Length == 0)
                return false;
            char cTestNo = TestNo[0];
            if ((cTestNo >= 48 && cTestNo <= 57) || (cTestNo >= 65 && cTestNo <= 90)
                || (cTestNo >= 97 && cTestNo <= 122))
                return true;
            else
                return false;
        }

        private bool IsNumber(string TestNo)
        {
            if (TestNo.Length == 0)
                return false;
            char cTestNo = TestNo[0];
            if (cTestNo >= 48 && cTestNo <= 57)
                return true;
            else
                return false;
        }

        private void btnParse_Click(object sender, EventArgs e)
        {
            SetVariables();
            ParseAll();
            Console.Write(PostFixString);
            txtResult.Text = PostFixString;
            txtValue.Text = Calculate();
        }

        private string Calculate()
        {
            for(int i = 0; i < PostFixQueue.Count; i++)
            {
                switch(PostFixQueue[i])
                {
                    case "+":
                        PostFixQueue[i] = (Convert.ToDouble(PostFixQueue[i - 2]) + Convert.ToDouble(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "-":
                        PostFixQueue[i] = (Convert.ToDouble(PostFixQueue[i - 2]) - Convert.ToDouble(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "/":
                        PostFixQueue[i] = (Convert.ToDouble(PostFixQueue[i - 2]) / Convert.ToDouble(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "\\":
                        PostFixQueue[i] = (Convert.ToInt32(PostFixQueue[i - 2]) / Convert.ToInt32(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "*":
                        PostFixQueue[i] = (Convert.ToDouble(PostFixQueue[i - 2]) * Convert.ToDouble(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "MOD":
                        PostFixQueue[i] = (Convert.ToInt32(PostFixQueue[i - 2]) % Convert.ToInt32(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 2);
                        PostFixQueue.RemoveAt(i - 2);
                        i -= 2;
                        break;
                    case "SQR":
                        PostFixQueue[i] = (Math.Sqrt(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "SQUARE":
                        PostFixQueue[i] = (Math.Pow(Convert.ToDouble(PostFixQueue[i - 1]), 2)).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "CUBE":
                        PostFixQueue[i] = (Math.Pow(Convert.ToDouble(PostFixQueue[i - 1]), 3)).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "INT":
                        PostFixQueue[i] = (Convert.ToInt32(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "FIX":
                        PostFixQueue[i] = (Convert.ToInt32(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "ROUND":
                        PostFixQueue[i] = (Math.Round(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "RND":
                        Random rnd = new Random();
                        int value = rnd.Next(0, Convert.ToInt32(PostFixQueue[i - 1]));
                        PostFixQueue[i] = value.ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "ABS":
                        PostFixQueue[i] = (Math.Abs(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "SGN":
                        PostFixQueue[i] = (Math.Sign(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "REPROC":
                        PostFixQueue[i] = (Math.Sign(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "INVSGN":
                        PostFixQueue[i] = (Convert.ToDouble(PostFixQueue[i - 1])).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "FACT":
                        break;
                    case "SIN":
                        PostFixQueue[i] = (Math.Sin(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "COS":
                        PostFixQueue[i] = (Math.Cos(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "TAN":
                        PostFixQueue[i] = (Math.Tan(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "SEC":
                        PostFixQueue[i] = (1.0 / Math.Cos(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "COSEC":
                        PostFixQueue[i] = (1.0 / Math.Sin(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "COT":
                        PostFixQueue[i] = (1.0 / Math.Tan(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "HSIN":
                        PostFixQueue[i] = (1.0 / Math.Sinh(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "HCOS":
                        PostFixQueue[i] = (1.0 / Math.Cosh(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "HTAN":
                        PostFixQueue[i] = (1.0 / Math.Tanh(Convert.ToDouble(PostFixQueue[i - 1]))).ToString();
                        PostFixQueue.RemoveAt(i - 1);
                        i -= 1;
                        break;
                    case "HSEC":
                        break;
                    case "HCOSEC":
                        break;
                    case "HCOT":
                        break;
                }
            }
            return PostFixQueue.Last();
        }
    }
}
