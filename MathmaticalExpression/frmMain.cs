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
        public int LexicalAttribute;
        public string LexicalAttribute2;
        public string PostFixString;
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
            ExprString = txtExpression.Text;
            PostFixString = "";
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
                            while (IsNumber(ExprString[LexPos - 1].ToString()))
                            {
                                Temp = Temp + ExprString[LexPos - 1].ToString();
                                LexPos++;
                                if (LexPos > ExprString.Length)
                                    break;
                            }
                            LexicalAttribute = Convert.ToInt32(Temp);
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

        private void AddToPostFix(string TokenVal, int TokenAttr)
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
                    break;
                case "NUM":
                case "ID":
                    PostFixString = PostFixString + TokenAttr.ToString() + " ";
                    break;
                default:
                    PostFixString = PostFixString + TokenVal + " ";
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
        }
    }
}
