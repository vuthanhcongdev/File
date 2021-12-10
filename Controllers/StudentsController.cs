using CRUD_With_File.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CRUD_File.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private static string dataText = System.IO.File.ReadAllText("data.txt");

        private List<Student> dataObjectList = JsonConvert.DeserializeObject<List<Student>>(dataText);

        private const string path = "data.txt";

        [HttpGet]
        public List<Student> Get()
        {
            string lastestDataText = System.IO.File.ReadAllText("data.txt");

            List<Student> latestDataObject = System.Text.Json.JsonSerializer.Deserialize<List<Student>>(lastestDataText);
            return latestDataObject;
        }

        // https://localhost:44385/api/students/getbyid?studentId=1&name=ref
        [HttpGet("getbyid")]
        public Student GetById([FromQuery] int studentId, [FromQuery] string name)
        {
            var student = dataObjectList.Find(x => x.Id == studentId && x.Name == name);
            if (student == null)
            {
                return null;
            }
            return student;
        }

        // https://localhost:44385/api/students/getbyanobject?Id=1&name=ref&gender=1&class=32
        [HttpGet("getbyanobject")]
        public Student GetByAnObject([FromQuery] Student studentEntity)
        {
            var student = dataObjectList.Find(x => x.Id == studentEntity.Id && x.Name == studentEntity.Name);
            if (student == null)
            {
                return null;
            }
            return student;
        }

        [HttpPost]
        public List<Student> Post([FromBody] StudentDto studentDto)
        {
            var student = new Student();
            var maxId = dataObjectList.Max(x => x.Id);
            student.Id = maxId + 1;
            student.Name = studentDto.Name;
            student.Gender = studentDto.Gender;
            student.Class = studentDto.Class;
            dataObjectList.Add(student);
            var newStudentJson = JsonConvert.SerializeObject(dataObjectList, Formatting.Indented);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, String.Empty);
                System.IO.File.AppendAllText(path, newStudentJson);
            }
            return dataObjectList;
        }

        [HttpPut("{studentId:int}")]
        public IActionResult Put(int studentId, [FromForm] StudentDto studentDto)
        {
            var student = dataObjectList.Find(x => x.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }
            dataObjectList.Remove(student);
            student.Name = studentDto.Name;
            student.Gender = studentDto.Gender;
            student.Class = studentDto.Class;
            dataObjectList.Add(student);
            var newList = dataObjectList.OrderBy(x => x.Id).ToList();
            var newStudentJson = JsonConvert.SerializeObject(newList, Formatting.Indented);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, String.Empty);
                System.IO.File.AppendAllText(path, newStudentJson);
            }
            return Ok(newList);
        }

        [HttpDelete("{studentId:int}")]
        public IActionResult Delete(int studentId)
        {
            var student = dataObjectList.Find(x => x.Id == studentId);
            if (student == null)
            {
                return NotFound();
            }
            dataObjectList.Remove(student);
            var newStudentJson = JsonConvert.SerializeObject(dataObjectList, Formatting.Indented);
            if (System.IO.File.Exists(path))
            {
                System.IO.File.WriteAllText(path, String.Empty);
                System.IO.File.AppendAllText(path, newStudentJson);
            }
            return Ok(dataObjectList);
        }
    }
}