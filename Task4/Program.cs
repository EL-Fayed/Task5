using System;
using System.Collections.Generic;
using System.Linq;

namespace Task4
{
    public enum QuestionLevel { Easy = 1, Medium, Hard }
    public enum ExamType { Practical = 1, Final }

    public abstract class Question
    {
        public string Header { get; set; }
        public int Marks { get; set; }
        public QuestionLevel Level { get; set; }

        protected Question(string header, int marks, QuestionLevel level)
        {
            Header = header;
            Marks = marks;
            Level = level;
        }

        public abstract void Display();
        public abstract bool CheckAnswer(string userAnswer);
    }

    public class TrueFalseQuestion : Question
    {
        public bool CorrectAnswer { get; set; }

        public TrueFalseQuestion(string header, int marks, QuestionLevel level, bool correctAnswer)
            : base(header, marks, level)
        {
            CorrectAnswer = correctAnswer;
        }

        public override void Display()
        {
            Console.WriteLine($"[True/False] {Header} (Marks: {Marks})");
            Console.WriteLine("1. True");
            Console.WriteLine("2. False");
        }

        public override bool CheckAnswer(string userAnswer)
        {
            if (bool.TryParse(userAnswer, out bool parsedAnswer))
            {
                return parsedAnswer == CorrectAnswer;
            }
            if (userAnswer.Trim() == "1" && CorrectAnswer == true) return true;
            if (userAnswer.Trim() == "2" && CorrectAnswer == false) return true;

            return false;
        }
    }

    public class ChooseOneQuestion : Question
    {
        public List<string> Choices { get; set; }
        public int CorrectChoiceIndex { get; set; } 

        public ChooseOneQuestion(string header, int marks, QuestionLevel level, List<string> choices, int correctChoiceIndex)
            : base(header, marks, level)
        {
            Choices = choices;
            CorrectChoiceIndex = correctChoiceIndex;
        }

        public override void Display()
        {
            Console.WriteLine($"[Choose One] {Header} (Marks: {Marks})");
            for (int i = 0; i < Choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Choices[i]}");
            }
        }

        public override bool CheckAnswer(string userAnswer)
        {
            if (int.TryParse(userAnswer.Trim(), out int choice))
            {
                return choice == CorrectChoiceIndex;
            }
            return false;
        }
    }
    public class MultipleChoiceQuestion : Question
    {
        public List<string> Choices { get; set; }
        public List<int> CorrectChoices { get; set; } 

        public MultipleChoiceQuestion(string header, int marks, QuestionLevel level, List<string> choices, List<int> correctChoices)
            : base(header, marks, level)
        {
            Choices = choices;
            CorrectChoices = correctChoices;
        }

        public override void Display()
        {
            Console.WriteLine($"[Multiple Choice - Select all correct separated by commas, e.g., 1,3] {Header} (Marks: {Marks})");
            for (int i = 0; i < Choices.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {Choices[i]}");
            }
        }

        public override bool CheckAnswer(string userAnswer)
        {
            try
            {
                var userAnswers = userAnswer.Split(',')
                                            .Select(s => int.Parse(s.Trim()))
                                            .OrderBy(x => x)
                                            .ToList();

                var sortedCorrect = CorrectChoices.OrderBy(x => x).ToList();

                return userAnswers.SequenceEqual(sortedCorrect);
            }
            catch
            {
                return false;
            }
        }
    }
    class Program
    {        private static List<Question> questionBank = new List<Question>();

        static void Main(string[] args)
        {
            SeedData();

            while (true)
            {
                Console.Clear();
                Console.WriteLine("     EXAMINATION SYSTEM - MAIN MENU   ");
                Console.WriteLine("======================================");
                Console.WriteLine("1. Doctor Mode");
                Console.WriteLine("2. Student Mode");
                Console.WriteLine("3. Exit");
                Console.Write("Select an option (1-3): ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        DoctorMode();
                        break;
                    case "2":
                        StudentMode();
                        break;
                    case "3":
                        Console.WriteLine("Exiting program. Good bye!");
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Press any key to try again.");
                        Console.ReadKey();
                        break;
                }
            }
        }

        private static void DoctorMode()
        {
            Console.Clear();
            Console.WriteLine("--- Doctor Mode: Question Bank Setup ---");
            Console.Write("Enter number of questions to add: ");
            if (!int.TryParse(Console.ReadLine(), out int count) || count <= 0)
            {
                Console.WriteLine("Invalid number. Press any key to return.");
                Console.ReadKey();
                return;
            }

            for (int i = 0; i < count; i++)
            {
                Console.WriteLine($"\n--- Entering Question {i + 1} of {count} ---");
                Console.WriteLine("Select Question Type:");
                Console.WriteLine("1. True / False");
                Console.WriteLine("2. Choose One");
                Console.WriteLine("3. Multiple Choice");
                Console.Write("Choice: ");
                string typeChoice = Console.ReadLine();
                Console.WriteLine("Select Level (1. Easy, 2. Medium, 3. Hard): ");
                if (!Enum.TryParse(Console.ReadLine(), out QuestionLevel level))
                {
                    level = QuestionLevel.Easy; 
                }
                Console.Write("Enter Question Header: ");
                string header = Console.ReadLine();

                Console.Write("Enter Marks: ");
                int.TryParse(Console.ReadLine(), out int marks);

                if (typeChoice == "1")
                {
                    Console.Write("Enter Correct Answer (True/False or 1/2): ");
                    string tfInput = Console.ReadLine();
                    bool correct = tfInput.Equals("true", StringComparison.OrdinalIgnoreCase) || tfInput == "1";

                    questionBank.Add(new TrueFalseQuestion(header, marks, level, correct));
                }
                else if (typeChoice == "2")
                {
                    List<string> choices = new List<string>();
                    for (int c = 1; c <= 4; c++)
                    {
                        Console.Write($"Enter choice {c}: ");
                        choices.Add(Console.ReadLine());
                    }
                    Console.Write("Enter correct choice number (1-4): ");
                    int.TryParse(Console.ReadLine(), out int correctIdx);

                    questionBank.Add(new ChooseOneQuestion(header, marks, level, choices, correctIdx));
                }
                else if (typeChoice == "3")
                {
                    List<string> choices = new List<string>();
                    for (int c = 1; c <= 4; c++)
                    {
                        Console.Write($"Enter choice {c}: ");
                        choices.Add(Console.ReadLine());
                    }
                    Console.Write("Enter correct answer choice numbers separated by comma (e.g., 1,3): ");
                    string[] parts = Console.ReadLine().Split(',');
                    List<int> correctIndices = new List<int>();
                    foreach (var p in parts)
                    {
                        if (int.TryParse(p.Trim(), out int idx))
                            correctIndices.Add(idx);
                    }

                    questionBank.Add(new MultipleChoiceQuestion(header, marks, level, choices, correctIndices));
                }
                else
                {
                    Console.WriteLine("Invalid type selected. Question skipped.");
                    i--;
                }
            }

            Console.WriteLine("\nQuestions added successfully! Press any key to return to Main Menu.");
            Console.ReadKey();
        }

        private static void StudentMode()
        {
            Console.Clear();
            if (questionBank.Count == 0)
            {
                Console.WriteLine("No questions available in the Question Bank! Please enter questions in Doctor Mode first.");
                Console.WriteLine("Press any key to return.");
                Console.ReadKey();
                return;
            }

            Console.WriteLine("--- Student Mode ---");
            Console.WriteLine("Select Exam Type:");
            Console.WriteLine("1. Practical Exam");
            Console.WriteLine("2. Final Exam");
            Console.Write("Choice: ");
            if (!Enum.TryParse(Console.ReadLine(), out ExamType examType))
            {
                examType = ExamType.Final;
            }

            Console.WriteLine("Select Level:");
            Console.WriteLine("1. Easy");
            Console.WriteLine("2. Medium");
            Console.WriteLine("3. Hard");
            Console.Write("Choice: ");
            if (!Enum.TryParse(Console.ReadLine(), out QuestionLevel level))
            {
                level = QuestionLevel.Easy;
            }

             var filteredQuestions = questionBank.Where(q => q.Level == level).ToList();

            if (filteredQuestions.Count == 0)
            {
                Console.WriteLine($"No questions found for level '{level}'. Press any key to return.");
                Console.ReadKey();
                return;
            }

            if (examType == ExamType.Practical)
            {
                int countToTake = Math.Max(1, filteredQuestions.Count / 2);
                filteredQuestions = filteredQuestions.Take(countToTake).ToList();
            }

            int totalMarks = filteredQuestions.Sum(q => q.Marks);
            int earnedMarks = 0;

            Console.Clear();
            Console.WriteLine($"Starting {examType} Exam ({level} Level). Total Questions: {filteredQuestions.Count}\n");

            for (int i = 0; i < filteredQuestions.Count; i++)
            {
                Console.WriteLine($"Question {i + 1}:");
                filteredQuestions[i].Display();
                Console.Write("Your Answer: ");
                string userAnswer = Console.ReadLine();

                bool isCorrect = filteredQuestions[i].CheckAnswer(userAnswer);
                if (isCorrect)
                {
                    earnedMarks += filteredQuestions[i].Marks;
                    Console.WriteLine("-> Correct!\n");
                }
                else
                {
                    Console.WriteLine("-> Incorrect.\n");
                }
            }
            Console.WriteLine("======================================");
            Console.WriteLine($" Your Result: {earnedMarks}/{totalMarks} ");
            Console.WriteLine("======================================");
            Console.WriteLine("Press any key to return to Main Menu.");
            Console.ReadKey();
        }

        private static void SeedData()
        {
            questionBank.Add(new TrueFalseQuestion("C# is an object-oriented programming language.", 5, QuestionLevel.Easy, true));
            questionBank.Add(new ChooseOneQuestion("Which keyword is used to define a class in C#?", 5, QuestionLevel.Easy, new List<string> { "struct", "class", "interface", "method" }, 2));
            questionBank.Add(new ChooseOneQuestion("Which keyword is used to define a class in C#?", 5, QuestionLevel.Hard, new List<string> { "struct", "class", "interface", "method" }, 2));
            questionBank.Add(new MultipleChoiceQuestion("Which of the following are access modifiers in C#?", 10, QuestionLevel.Medium, new List<string> { "public", "private", "fast", "protected" }, new List<int> { 1, 2, 4 }));
            questionBank.Add(new MultipleChoiceQuestion("Which of the following are access modifiers in C#?", 10, QuestionLevel.Hard, new List<string> { "public", "private", "fast", "protected" }, new List<int> { 1, 2, 4 }));
        }
    }
}