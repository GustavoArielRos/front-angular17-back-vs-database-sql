using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TodoController : ControllerBase
    {

        private IConfiguration _configuration;
        public TodoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet("get_tasks")]//pegar os dados
        public JsonResult get_tasks()//método que pega todos os noms da table
        {
            string query = "select * from todo";//criando uma query
            DataTable table = new DataTable();//para armazenar os dados da tabela
            string SqlDatasource = _configuration.GetConnectionString("mydb");//variavel que tem a conection string
            SqlDataReader myReader;
            using(SqlConnection myCon = new SqlConnection(SqlDatasource))//usando a classe de sql para conectar com a base de dados do server
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);//filtra o resultado
                   
                }

                return new JsonResult(table);
            }
        }

        //Define um endpoint HTTP POST com a rota "add_task"
        [HttpPost("add_task")]
        public JsonResult add_task([FromForm] string task)//método add_task que recebe um parâmetro task do formulário HTTP
        {   
            //query sql para inserir um novo valor na tabela 'todo'
            string query = "insert into todo values (@task)";//Usando o parâmetro task(task é o valor que será inserido) para evitr SQL Injection(impedir ataques)
            DataTable table = new DataTable();//criando tabela para armazenar resultados
            string SqlDatasource = _configuration.GetConnectionString("mydb");//se conectando com o banco de dados através da conection string
            SqlDataReader myReader;//leitor sql para ler os dados do banco

            //Criando uma nova conexão sql usando a stringconnection
            using (SqlConnection myCon = new SqlConnection(SqlDatasource))
            {
                //Abre a conexão com o banco de dados
                myCon.Open();
                //Cria um novo comando SQL usando a query e a conexão aberta
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {   
                    //adiciona o parâmetro 'task' ao comando SQL
                    myCommand.Parameters.AddWithValue("@task", task);
                    myReader = myCommand.ExecuteReader();//executa o comando e obtém um leitor de dados
                    table.Load(myReader);//carrega os dados lidos para a tabela em memoria
                }
            }

            return new JsonResult("Added Successfully");//retorna o json indicando que  foi um sucesso
        }

        //endpoint POST com a rota "delete_task"(mesmo tendo post ele faz a função de delete por causa do sql)
        [HttpPost("delete_task")]

        public JsonResult delete_task([FromForm] string id)
        {
            string query = "delete from todo where id=@id";//perceba que para referência a variável do parâmetro eu uso o @
            DataTable table = new DataTable();
            string SqlDatasource = _configuration.GetConnectionString("mydb");
            SqlDataReader myReader;

            using (SqlConnection myCon = new SqlConnection(SqlDatasource))
            {
                myCon.Open();
                using(SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myCommand.Parameters.AddWithValue("@id", id);
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                }
            }

            return new JsonResult("Deleted Successfully");
        }


    }
}
