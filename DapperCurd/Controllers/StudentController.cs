using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace DapperCurd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentController : ControllerBase
    {
        private readonly IConfiguration _config;

        public StudentController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<ActionResult<List<Student>>> GetAllStudents()
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            IEnumerable<Student> student = await SelectAllStudent(connection);
            return Ok(student);
        }


        [HttpGet("{studentId}")]
        public async Task<ActionResult<Student>> GetStudent(int studentId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            var student = await connection.QueryFirstAsync<Student>("select * from students where studentid = @StudentID",
                new { StudentID = studentId});
            return Ok(student);
        }


        [HttpPost]
        public async Task<ActionResult<List<Student>>> CreateStudent(Student student)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("insert into students (fullname,emailaddress,city) values (@FullName, @EmailAddress, @City)", student);
            return Ok(await SelectAllStudent(connection));
        }


        [HttpPut]
        public async Task<ActionResult<List<Student>>> UpdateStudent(Student student)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("update students set fullname=@FullName, emailaddress=@EmailAddress, city=@City where studentid = @StudentID", student);
            return Ok(await SelectAllStudent(connection));
        }



        [HttpDelete("{studentId}")]
        public async Task<ActionResult<List<Student>>> DeleteStudent(int studentId)
        {
            using var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            await connection.ExecuteAsync("delete from students where studentid = @StudentID", new { StudentID = studentId });
            return Ok(await SelectAllStudent(connection));
        }


        private static async Task<IEnumerable<Student>> SelectAllStudent(SqlConnection connection)
        {
            return await connection.QueryAsync<Student>("select * from students");
        }
    }
}
