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
        public ActionResult displayCreateStrawPoll()
        {
            return View("CreateStrawPoll");
        }

        //CreateStrawPoll
        public ActionResult CreateStrawPoll(String question, String answerOne, String answerTwo, String answerThree, String answerFour, bool questionChoice)
        {
            //Create question
            Models.StrawPoll myStrawPoll = new Models.StrawPoll(question, questionChoice, 0);

            //Create answers
            Models.Answer myAnswerOne = new Models.Answer(answerOne, 0);
            Models.Answer myAnswerTwo = new Models.Answer(answerTwo, 0);
            Models.Answer myAnswerThree = new Models.Answer(answerThree, 0);
            Models.Answer myAnswerFour = new Models.Answer(answerFour, 0);

            //Connection BDD
            Console.WriteLine("test OK");

            //TODO: send BDD (link btw question and answers ?? --> TODO)

            //TODO : return URL page (??)
            return View("URLGeneration");
        }

        //METHODS//
        public void addStrawPollInDataBase(StrawPoll newStrawPoll)
        {
            String queryAddStrawPoll;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            //TODO - ADD URLs String before returning String

            queryAddStrawPoll = "INSERT INTO Sondage(ChoixMultiple, QuestionSondage, " +
                "NbrVotantsSondage, URLSondage, URLSuppressionSondage, URLResultatSondage) " +
                "VALUES(@ChoixMultiple, @QuestionSondage, @NbrVotantsSondage, @URLSondage" +
                "@URLSuppressionSondage, @URLResultatSondage)";

            newDataBaseTask.OpenConnection();
            SqlCommand cmd = new SqlCommand(queryAddStrawPoll);
            cmd.Parameters.AddWithValue("@ChoixMultiple", newStrawPoll.getMultipleChoices());
            cmd.Parameters.AddWithValue("@QuestionSondage", newStrawPoll.getStrawPollQuestion());
            cmd.Parameters.AddWithValue("@NbrVotantsSondage", newStrawPoll.getNbrVotesStrawPoll());
            cmd.Parameters.AddWithValue("@URLSondage", newStrawPoll.getURLStrawPoll());
            cmd.Parameters.AddWithValue("@URLSuppressionSondage", newStrawPoll.getURLSuppression());
            cmd.Parameters.AddWithValue("@URLResultatSondage", newStrawPoll.getURLResults());

            newDataBaseTask.setMySqlCommand(cmd);
            newDataBaseTask.ExecuteNonQuery();
            newDataBaseTask.CloseConnection();
        }
    }
}