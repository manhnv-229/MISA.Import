using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.ImportDemo.Core.Interfaces;

namespace MISA.ImportDemo.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DictionarysController : ControllerBase
    {
        IDictionaryEnumService _service;
        public DictionarysController(IDictionaryEnumService service)
        {
            _service = service;
        }

        [HttpGet("gender")]
        public IActionResult GetGender()
        {
            var data = _service.GetGenders();
            return Ok(data);
        }

        [HttpGet("Trainning-Levels")]
        public IActionResult GetTrainningLevels()
        {
            var data = _service.GetTrainningLevels();
            return Ok(data);
        }

        [HttpGet("Work-Status")]
        public IActionResult GetWorkStatus()
        {
            var data = _service.GetWorkStatus();
            return Ok(data);
        }

        [HttpGet("Marital-Status")]
        public IActionResult GetMaritalStatus()
        {
            var data = _service.GetMaritalStatus();
            return Ok(data);
        }

        [HttpGet("Degree-Classifications")]
        public IActionResult GetDegreeClassifications()
        {
            var data = _service.GetDegreeClassifications();
            return Ok(data);
        }

        [HttpGet("exercise-types")]
        public IActionResult GetExerciseTypes()
        {
            var data = _service.GetExerciseTypes();
            return Ok(data);
        }

        [HttpGet("question-types")]
        public IActionResult GetQuestionType()
        {
            var data = _service.GetQuestionType();
            return Ok(data);
        }

        [HttpGet("video-types")]
        public IActionResult GetVideoTypes()
        {
            var data = _service.GetVideoTypes();
            return Ok(data);
        }

        [HttpGet("thread-types")]
        public IActionResult GetThreadTypes()
        {
            var data = _service.GetThreadTypes();
            return Ok(data);
        }
    }
}
