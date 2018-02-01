using GuldStrawPoll.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.SqlClient;

namespace GuldStrawPoll.Controllers
{
    public class HomeController : Controller
    {
        // Display welcome page
        public ActionResult Index()
        {
            ViewBag.Nom = "Mon p'tit Guld !";
                return View("Index");
        }

        //Display Create Page
        public ActionResult displayCreatePage()
        {
            return View("CreateStrawPoll");
        }

        //CreateStrawPoll
        public ActionResult CreateStrawPoll(String question, String answerOne, String answerTwo, String answerThree, String answerFour, String questionChoice)
        {
            bool multipleChoices;
            if(questionChoice == "uniqueChoice")
            {
                multipleChoices = false;
            }
            else
            {
                multipleChoices = true;
            }
            //Create question
            Models.StrawPoll myStrawPoll = new Models.StrawPoll(question, multipleChoices, 0, false);

            //Create answers
            Models.Answer myAnswerOne = new Models.Answer(answerOne, 0);
            Models.Answer myAnswerTwo = new Models.Answer(answerTwo, 0);
            Models.Answer myAnswerThree = new Models.Answer(answerThree, 0);
            Models.Answer myAnswerFour = new Models.Answer(answerFour, 0);

            //Send StrawPoll in database
            int IDStrawPoll = addStrawPollInDataBase(myStrawPoll);

            //Send URL in database
            sendURLsInDataBase(myStrawPoll, IDStrawPoll);

            //Send Answers in database and assign them to the corresponding strawpoll
            sendAnswerInDatabase(myAnswerOne, IDStrawPoll);
            sendAnswerInDatabase(myAnswerTwo, IDStrawPoll);
            sendAnswerInDatabase(myAnswerThree, IDStrawPoll);
            sendAnswerInDatabase(myAnswerFour, IDStrawPoll);

            ViewBag.URLStrawPoll = myStrawPoll.getURLStrawPoll();
            ViewBag.URLDeletion = myStrawPoll.getURLDeletion();
            ViewBag.URLResult = myStrawPoll.getURLResults();

            //return URL page
            return View("URLGeneration");
        }

        //Access vote page
        public ActionResult VotePage(int ID)
        {
            Models.StrawPoll myStrawPoll = new Models.StrawPoll();
            Models.Answer myAnswerOne = new Models.Answer();
            Models.Answer myAnswerTwo = new Models.Answer();
            Models.Answer myAnswerThree = new Models.Answer();
            Models.Answer myAnswerFour = new Models.Answer();

            //Get strawpoll in database
            String queryGetStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryGetStrawPoll = "SELECT * FROM StrawPoll WHERE NumStrawPoll = @ID;";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryGetStrawPoll, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@ID", ID);

            SqlDataReader newReader = cmd.ExecuteReader();
            while (newReader.Read())
            {
                myStrawPoll.setStrawPollQuestion((String)newReader["Question"]);
                myStrawPoll.setMultipleChoices((bool)newReader["MultipleChoices"]);
                myStrawPoll.setNbrVotesStrawPoll((int)newReader["NbrVotes"]);
                myStrawPoll.setURLStrawPoll((String)newReader["URLStrawPoll"]);
                myStrawPoll.setURLDeletion((String)newReader["URLDeletion"]);
                myStrawPoll.setURLResults((String)newReader["URLResult"]);
                myStrawPoll.setIsActive((bool)newReader["IsActive"]);
            }

            newReader.Close();

            String queryGetAnswers = "SELECT * FROM Answer WHERE NumStrawPoll = @ID;";

            SqlCommand cmdAnswer = new SqlCommand(queryGetAnswers, newDataBaseTask.getSqlConnection());
            cmdAnswer.Parameters.AddWithValue("@ID", ID);

            SqlDataReader newReaderAnswer = cmdAnswer.ExecuteReader();

            List<Models.Answer> answers = new List<Models.Answer>();

            while (newReaderAnswer.HasRows)
            {

                while (newReaderAnswer.Read())
                {
                    Models.Answer newAnswer = new Models.Answer((String)newReaderAnswer["Answer"], (int)newReaderAnswer["NbrVotes"]);
                    answers.Add(newAnswer);
                }
                newReaderAnswer.NextResult();
            }

            newReaderAnswer.Close();
            newDataBaseTask.CloseConnection();

            myAnswerOne = answers.ElementAt(0);
            myAnswerTwo = answers.ElementAt(1);
            myAnswerThree = answers.ElementAt(2);
            myAnswerFour = answers.ElementAt(3);

            //SEND TO THE VIEW
            ViewBag.StrawPoll = myStrawPoll;
            ViewBag.answerOne = myAnswerOne;
            ViewBag.answerTwo = myAnswerTwo;
            ViewBag.answerThree = myAnswerThree;
            ViewBag.answerFour = myAnswerFour;

            ViewBag.IDStrawPoll = ID;

            if(myStrawPoll.getIsActive() == false)
            {
                return View("Error_404");
            }
            else if(myStrawPoll.getMultipleChoices() == false)
            {
                return View("UniqueChoice");
            }
            else
            {
                return View("MultipleChoices");
            }
        }

        //Delete vote page
        public ActionResult DeletionPage(Guid GuidStrawPoll)
        {
            String queryDisableStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryDisableStrawPoll = "UPDATE StrawPoll " +
                "SET IsActive = @IsActive " +
                "WHERE Guid = @GUID";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryDisableStrawPoll, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@IsActive", false);
            cmd.Parameters.AddWithValue("@GUID", GuidStrawPoll);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();

            return View("Index");
        }

        //Access result page
        public ActionResult ResultPage(int ID)
        {
            Models.StrawPoll myStrawPoll = new Models.StrawPoll();
            Models.Answer myAnswerOne = new Models.Answer();
            Models.Answer myAnswerTwo = new Models.Answer();
            Models.Answer myAnswerThree = new Models.Answer();
            Models.Answer myAnswerFour = new Models.Answer();

            //Get strawpoll in database
            String queryGetStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryGetStrawPoll = "SELECT * FROM StrawPoll WHERE NumStrawPoll = @ID;";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryGetStrawPoll, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@ID", ID);

            SqlDataReader newReader = cmd.ExecuteReader();
            while (newReader.Read())
            {
                myStrawPoll.setStrawPollQuestion((String)newReader["Question"]);
                myStrawPoll.setMultipleChoices((bool)newReader["MultipleChoices"]);
                myStrawPoll.setNbrVotesStrawPoll((int)newReader["NbrVotes"]);
                myStrawPoll.setURLStrawPoll((String)newReader["URLStrawPoll"]);
                myStrawPoll.setURLDeletion((String)newReader["URLDeletion"]);
                myStrawPoll.setURLResults((String)newReader["URLResult"]);
                myStrawPoll.setIsActive((bool)newReader["IsActive"]);
            }

            newReader.Close();

            String queryGetAnswers = "SELECT * FROM Answer WHERE NumStrawPoll = @ID;";

            SqlCommand cmdAnswer = new SqlCommand(queryGetAnswers, newDataBaseTask.getSqlConnection());
            cmdAnswer.Parameters.AddWithValue("@ID", ID);

            SqlDataReader newReaderAnswer = cmdAnswer.ExecuteReader();

            List<Models.Answer> answers = new List<Models.Answer>();

            while (newReaderAnswer.HasRows)
            {

                while (newReaderAnswer.Read())
                {
                    Models.Answer newAnswer = new Models.Answer((String)newReaderAnswer["Answer"], (int)newReaderAnswer["NbrVotes"]);
                    answers.Add(newAnswer);
                }
                newReaderAnswer.NextResult();
            }

            newReaderAnswer.Close();
            newDataBaseTask.CloseConnection();

            //Set answers with database datas
            myAnswerOne = answers.ElementAt(0);
            myAnswerTwo = answers.ElementAt(1);
            myAnswerThree = answers.ElementAt(2);
            myAnswerFour = answers.ElementAt(3);

            //SEND TO THE VIEW
            ViewBag.StrawPoll = myStrawPoll;

            ViewBag.aOne = myAnswerOne.getAnswer();
            ViewBag.vOne = myAnswerOne.getNbrVotesByAnswer();
            ViewBag.aTwo = myAnswerTwo.getAnswer();
            ViewBag.vTwo = myAnswerTwo.getNbrVotesByAnswer();
            ViewBag.aThree = myAnswerThree.getAnswer();
            ViewBag.vThree = myAnswerThree.getNbrVotesByAnswer();
            ViewBag.aFour = myAnswerFour.getAnswer();
            ViewBag.vFour = myAnswerFour.getNbrVotesByAnswer();

            return View("Result");
        }

        public void VoteRadioButtons(int ID, String answer)
        {
            //INCREMENT DATABASE
            String queryIncrementNbrVotes;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryIncrementNbrVotes = "UPDATE StrawPoll " +
                "SET NbrVotes = NbrVotes + 1 " +
                "WHERE NumStrawPoll=@ID; " +
                "UPDATE Answer " +
                "SET NbrVotes = NbrVotes + 1 " +
                "WHERE NumStrawPoll = @IDAnswer AND Answer = @LabelAnswer;";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryIncrementNbrVotes, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@ID", ID);
            cmd.Parameters.AddWithValue("@IDAnswer", ID);
            cmd.Parameters.AddWithValue("@LabelAnswer", answer);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();

            newDataBaseTask.CloseConnection();

            //RETURN RESULT
            ResultPage(ID);
        }


        public int addStrawPollInDataBase(Models.StrawPoll newStrawPoll)
        {
            String queryAddStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryAddStrawPoll = "INSERT INTO StrawPoll(MultipleChoices, Question, NbrVotes, isActive, GUID, NumCreator) " +
                "VALUES(@MultipleChoices, @StrawPollQuestion, @NbrVotesStrawPoll, @IsActive, @GUID, @NumCreator); " +
                "SELECT scope_identity()";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddStrawPoll, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@MultipleChoices", newStrawPoll.getMultipleChoices());
            cmd.Parameters.AddWithValue("@StrawPollQuestion", newStrawPoll.getStrawPollQuestion());
            cmd.Parameters.AddWithValue("@NbrVotesStrawPoll", newStrawPoll.getNbrVotesStrawPoll());
            cmd.Parameters.AddWithValue("@IsActive", newStrawPoll.getIsActive());
            cmd.Parameters.AddWithValue("@GUID", newStrawPoll.getGuidStrawPoll());

            bool connected = false;
            if(!connected)
            {
                cmd.Parameters.AddWithValue("@NumCreator", DBNull.Value);
            }
            else
            {
                cmd.Parameters.AddWithValue("@NumCreator", 4);
            }

            int ID = Convert.ToInt32(cmd.ExecuteScalar());

            newDataBaseTask.CloseConnection();

            return ID;
        }

        public void sendURLsInDataBase(Models.StrawPoll lastStrawPoll, int ID)
        {
            String queryAddURLs;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            String URLStrawPoll = lastStrawPoll.generateURLStrawPoll(ID);
            lastStrawPoll.setURLStrawPoll(URLStrawPoll);

            String URLDeletion = lastStrawPoll.generateURLDeletion();
            lastStrawPoll.setURLDeletion(URLDeletion);

            String URLResult = lastStrawPoll.generateURLResults(ID);
            lastStrawPoll.setURLResults(URLResult);

            queryAddURLs = "UPDATE StrawPoll " +
                "SET URLStrawPoll = @URLStrawPoll, URLDeletion = @URLDeletion, URLResult = @URLResult, IsActive = @IsActive " +
                "WHERE NumStrawPoll=@ID";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddURLs, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@URLStrawPoll", URLStrawPoll);
            cmd.Parameters.AddWithValue("@URLDeletion", URLDeletion);
            cmd.Parameters.AddWithValue("@URLResult", URLResult);
            cmd.Parameters.AddWithValue("@IsActive", true);
            cmd.Parameters.AddWithValue("@ID", ID);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();
        }

        public void sendAnswerInDatabase(Models.Answer newAnswer, int ID)
        {
            String queryAddAnswer;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryAddAnswer = "INSERT INTO Answer(Answer, NbrVotes, NumStrawPoll) " +
                "VALUES(@Answer, @NbrVotes, @NumStrawPoll)";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddAnswer, newDataBaseTask.getSqlConnection());
            cmd.Parameters.AddWithValue("@Answer", newAnswer.getAnswer());
            cmd.Parameters.AddWithValue("@NbrVotes", newAnswer.getNbrVotesByAnswer());
            cmd.Parameters.AddWithValue("@NumStrawPoll", ID);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();
        }

        //Handle Cookies

        
    }
}