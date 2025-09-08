CREATE TABLE dbo.FrequentlyAskedQuestion(
	FrequentlyAskedQuestionID int NOT NULL IDENTITY(1,1) CONSTRAINT PK_FrequentlyAskedQuestion_FrequentlyAskedQuestionID PRIMARY KEY,
	QuestionText varchar(max) NOT NULL,
	AnswerText [dbo].[html] NOT NULL
);