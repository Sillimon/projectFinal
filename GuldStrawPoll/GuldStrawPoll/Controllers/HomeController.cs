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
        //ACTION RESULT//

        // Display welcome page
        public ActionResult Index()
        {
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
            if(questionChoice == "uniqueCHoice")
            {
                multipleChoices = false;
            }
            else
            {
                multipleChoices = true;
            }
            //Create question
            Models.StrawPoll myStrawPoll = new Models.StrawPoll(question, multipleChoices, 0);

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

            //return URL page
            return View("URLGeneration");
        }

        public ActionResult VotePage(int ID)//parameters inside function --> ?ID=5
        {
            //get corresponding strawpoll and answers in database and create object
            Models.StrawPoll myStrawPoll = new Models.StrawPoll();
            Models.Answer myAnswerOne = new Models.Answer();
            Models.Answer myAnswerTwo = new Models.Answer();
            Models.Answer myAnswerThree = new Models.Answer();
            Models.Answer myAnswerFour = new Models.Answer();

            //TODO - Get Values in Database and set objects below's attributes

            //SEND TO THE VIEW
            ViewBag.StrawPoll = myStrawPoll;
            ViewBag.answerOne = myAnswerOne;
            ViewBag.answerTwo = myAnswerTwo;
            ViewBag.answerThree = myAnswerThree;
            ViewBag.answerFour = myAnswerFour;

            return View();
        }

        //METHODS//
        public int addStrawPollInDataBase(Models.StrawPoll newStrawPoll)
        {
            String queryAddStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            //Changer les noms
            queryAddStrawPoll = "INSERT INTO StrawPoll(multipleChoices, strawPollQuestion, NbrVotesStrawPoll,) " +
                "VALUES(@multipleChoices, @strawPollQuestion, @NbrVotesStrawPoll); " +
                "SELECT scope_identity()";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddStrawPoll);
            cmd.Parameters.AddWithValue("@multipleChoices", newStrawPoll.getMultipleChoices());
            cmd.Parameters.AddWithValue("@strawPollQuestion", newStrawPoll.getStrawPollQuestion());
            cmd.Parameters.AddWithValue("@NbrVotesStrawPoll", newStrawPoll.getNbrVotesStrawPoll());

            int ID = (int)cmd.ExecuteNonQuery();

            newDataBaseTask.CloseConnection();

            return ID;
        }

        public void sendURLsInDataBase(Models.StrawPoll lastStrawPoll, int ID)
        {
            String queryAddURLs;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            String URLStrawPoll = lastStrawPoll.generateURLStrawPoll(ID);
            lastStrawPoll.setURLStrawPoll(URLStrawPoll);

            String URLDeletion = lastStrawPoll.generateURLDeletion(ID);
            lastStrawPoll.setURLDeletion(URLDeletion);

            String URLResult = lastStrawPoll.generateURLResults(ID);
            lastStrawPoll.setURLResults(URLResult);

            //Changer les noms
            queryAddURLs = "UPDATE StrawPoll " +
                "SET URLStrawPoll = @URLStrawPoll, URLDeletion = @URLDeletion, URLResult = @URLResult)" +
                "WHERE NumStrawPoll=@ID";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddURLs);
            cmd.Parameters.AddWithValue("@URLStrawPoll", URLStrawPoll);
            cmd.Parameters.AddWithValue("@URLDeletion", URLDeletion);
            cmd.Parameters.AddWithValue("@URLResult", URLResult);
            cmd.Parameters.AddWithValue("@ID", ID);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();
        }

        public void sendAnswerInDatabase(Models.Answer newAnswer, int ID)
        {
            String queryAddAnswer;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            //Changer les noms
            queryAddAnswer = "INSERT INTO Answer(answer, nbrVotes, NumStrawPoll,) " +
                "VALUES(@answer, @nbrVotes, @NumStrawPoll)";

            newDataBaseTask.OpenConnection();

            SqlCommand cmd = new SqlCommand(queryAddAnswer);
            cmd.Parameters.AddWithValue("@answer", newAnswer.getAnswer());
            cmd.Parameters.AddWithValue("@nbrVotes", newAnswer.getNbrVotesByAnswer());
            cmd.Parameters.AddWithValue("@NumStrawPoll", ID);

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();
        }
    }
}