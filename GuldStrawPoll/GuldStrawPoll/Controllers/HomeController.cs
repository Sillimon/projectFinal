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
        /// <summary>
        /// Display the welcome page
        /// </summary>
        /// <returns>View Index.cshtml</returns>
        public ActionResult Index()
        {
            return View("Index");
        }


        /// <summary>
        /// Display the creation page
        /// </summary>
        /// <returns>View CreateStrawPoll.cshtml</returns>
        public ActionResult DisplayCreatePage()
        {
            return View("CreateStrawPoll");
        }


        /// <summary>
        /// Get the <paramref name="question"/>, <paramref name="answerOne"/>, <paramref name="answerTwo"/>,
        /// <paramref name="answerThree"/>, <paramref name="answerFour"/> and the <paramref name="questionChoice"/>,
        /// make objects out of it and add them to the database
        /// </summary>
        /// <param name="question">The label of the question</param>
        /// <param name="answerOne">The label of the first question</param>
        /// <param name="answerTwo">The label of the second question</param>
        /// <param name="answerThree">The label of the third question</param>
        /// <param name="answerFour">The label of the fourth question</param>
        /// <param name="questionChoice">The type of strawpoll (simple or multiple answers allowed)</param>
        /// <returns>View URLGeneration.cshtml</returns>
        public ActionResult CreateStrawPoll(String question, String answerOne, String answerTwo, String answerThree, String answerFour, String questionChoice)
        {
            bool multipleChoices;
            if (questionChoice == "uniqueChoice")
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


        /// <summary>
        /// Check if the strawpoll is either deleted or allready answered by the current user (using cookies).
        /// If not, display the vote page
        /// </summary>
        /// <param name="ID">The strawpoll's unique ID</param>
        /// <returns>
        ///     <list type="bullet">  
        ///         <item>
        ///             <description>View Error_404.cshtml</description>  
        ///         </item>
        ///         <item>
        ///             <description>View CookieError.cshtml</description>  
        ///         </item>
        ///         <item>
        ///             <description>View UniqueChoice.cshtml</description>  
        ///         </item>
        ///         <item>
        ///             <description>View MultipleChoices.cshtml</description>  
        ///         </item>
        ///     </list>
        /// </returns>
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

            //Send data to the views
            ViewBag.StrawPoll = myStrawPoll;
            ViewBag.answerOne = myAnswerOne;
            ViewBag.answerTwo = myAnswerTwo;
            ViewBag.answerThree = myAnswerThree;
            ViewBag.answerFour = myAnswerFour;

            ViewBag.IDStrawPoll = ID;

            bool hasVoted = getCookies(ID);

            if (myStrawPoll.getIsActive() == false)
            {
                return View("Error_404");
            }
            else if (hasVoted)
            {
                return View("CookieError");
            }
            else if (myStrawPoll.getMultipleChoices() == false)
            {
                return View("UniqueChoice");
            }
            else
            {
                return View("MultipleChoices");
            }
        }


        /// <summary>
        /// Disable a specific strawpoll's vote page using its <paramref name="GuidStrawPoll"/>
        /// and redirect to the welcome page
        /// </summary>
        /// <param name="GuidStrawPoll">The Globally Unique Identifier of the strawpoll</param>
        /// <returns>View Index.cshtml</returns>
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


        /// <summary>
        /// Display the result page of a specific strawpoll using its <paramref name="ID"/>
        /// </summary>
        /// <param name="ID">The ID of the strawpoll</param>
        /// <returns>View Result.cshtml</returns>
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

            //Send data to the views
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
        

        /// <summary>
        /// Increment the number of votes of the strawpoll and the answer chosen.
        /// Display the result page then (unique answer only).
        /// </summary>
        /// <param name="ID">The ID of the strawpoll</param>
        /// <param name="answer">The label of the answer chosen</param>
        /// <returns>View Result.cshtml</returns>
        public ActionResult VoteRadioButtons(int ID, String answer)
        {
            //Increment database
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

            createCookies(ID);


            Models.StrawPoll myStrawPoll = new Models.StrawPoll();
            Models.Answer myAnswerOne = new Models.Answer();
            Models.Answer myAnswerTwo = new Models.Answer();
            Models.Answer myAnswerThree = new Models.Answer();
            Models.Answer myAnswerFour = new Models.Answer();

            //Get strawpoll in database
            String queryGetStrawPoll = "SELECT * FROM StrawPoll WHERE NumStrawPoll = @ID;";

            newDataBaseTask.OpenConnection();

            cmd = new SqlCommand(queryGetStrawPoll, newDataBaseTask.getSqlConnection());

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

            //Set answers with database data
            myAnswerOne = answers.ElementAt(0);
            myAnswerTwo = answers.ElementAt(1);
            myAnswerThree = answers.ElementAt(2);
            myAnswerFour = answers.ElementAt(3);

            //Send data to the views
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


        /// <summary>
        /// Increment the number of votes of the strawpoll and the answers chosen.
        /// Display the result page then (multiple answers only).
        /// </summary>
        /// <param name="ID">The ID of the strawpoll</param>
        /// <param name="checkbox">A list of String holding the label of each chosen answer</param>
        /// <returns>View Result.cshtml</returns>
        public ActionResult VoteCheckBoxes(int ID, List<String> checkbox)
        {
            //Increment database
            String queryIncrementNbrVotes;
            ConnectionQuery newDataBaseTask = new ConnectionQuery();

            queryIncrementNbrVotes = "UPDATE StrawPoll " +
                "SET NbrVotes = NbrVotes + 1 " +
                "WHERE NumStrawPoll=@ID; " +
                "UPDATE Answer " +
                "SET NbrVotes = NbrVotes + 1 " +
                "WHERE NumStrawPoll = @IDAnswer AND Answer = @LabelAnswer;";

            newDataBaseTask.OpenConnection();
            
            foreach (String answerChosen in checkbox)
            {
                SqlCommand newcmd = new SqlCommand(queryIncrementNbrVotes, newDataBaseTask.getSqlConnection());

                newcmd.Parameters.AddWithValue("@ID", ID);
                newcmd.Parameters.AddWithValue("@IDAnswer", ID);
                newcmd.Parameters.AddWithValue("@LabelAnswer", answerChosen);

                newDataBaseTask.setMySqlCommand(newcmd);
                newDataBaseTask.ExecuteNonQuery();
            }

            newDataBaseTask.CloseConnection();
            
            createCookies(ID);

            //RETURN RESULT
            Models.StrawPoll myStrawPoll = new Models.StrawPoll();
            Models.Answer myAnswerOne = new Models.Answer();
            Models.Answer myAnswerTwo = new Models.Answer();
            Models.Answer myAnswerThree = new Models.Answer();
            Models.Answer myAnswerFour = new Models.Answer();

            //Get strawpoll in database
            String queryGetStrawPoll = "SELECT * FROM StrawPoll WHERE NumStrawPoll = @ID;";

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


        /// <summary>
        /// Add the <paramref name="newStrawPoll"/> in the database
        /// </summary>
        /// <remarks>URLs of the strawpoll ain't inserted</remarks>
        /// <param name="newStrawPoll">The strawpoll object to insert</param>
        /// <returns>The ID of the strawpoll</returns>
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
            if (!connected)
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


        /// <summary>
        /// Generate URLs of the <paramref name="lastStrawPoll"/> using its <paramref name="ID"/>
        /// and insert them in database
        /// </summary>
        /// <remarks>It ain't creating a new strawpoll, the concerned strawpoll is updated</remarks>
        /// <param name="lastStrawPoll">The strawpoll object holding the URLs</param>
        /// <param name="ID">The ID of the strawpoll</param>
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


        /// <summary>
        /// Insert the <paramref name="newAnswer"/> in database
        /// </summary>
        /// <param name="newAnswer">The answer object to insert</param>
        /// <param name="ID">The ID of the strawpoll linked with this answer</param>
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


        /// <summary>
        /// Create a cookie with the <paramref name="ID"/>
        /// </summary>
        /// <remarks>The cookie created is specific to the user and the strawpoll</remarks>
        /// <param name="ID">The ID of the strawpoll</param>
        public void createCookies(int ID)
        {
            HttpCookie userInfo = new HttpCookie("GuldStrawPoll?"+ID);
            userInfo["hasVoted"] = "True";
            userInfo.Expires = DateTime.Now.AddDays(100);
            Response.Cookies.Add(userInfo);
        }


        /// <summary>
        /// Check, for the current browser, if a cookie exist for a specific strawpoll,
        /// using its <paramref name="ID"/>
        /// </summary>
        /// <param name="ID">The ID of the strawpoll</param>
        /// <returns>
        ///     Boolean hasVoted
        ///     <list type="bullet">  
        ///         <item>
        ///             <description>True if the cookie allready exist</description>  
        ///         </item>
        ///         <item>
        ///             <description>False if the cookie doesn't exist yet</description>  
        ///         </item>
        ///     </list>
        /// </returns>
        public bool getCookies(int ID)
        {
            bool hasVoted;
            
            HttpCookie reqCookies = Request.Cookies["GuldStrawPoll?" + ID];

            if (reqCookies != null)
            {
                hasVoted = true;
            }
            else
            {
                hasVoted = false;
            }

            return hasVoted;
        }
    }
}