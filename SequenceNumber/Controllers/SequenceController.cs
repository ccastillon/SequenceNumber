using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

namespace SequenceNumber.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SequenceController : ControllerBase
    {
        // Thread-safe dictionary to store sequence numbers for each client
        private static ConcurrentDictionary<string, int> clientSequences = new ConcurrentDictionary<string, int>();

        // Lock object to synchronize access to sequence numbers
        private static object lockObj = new object();

        [HttpGet("getnextid")]
        public ActionResult<int> GetNextId([FromQuery] string clientid)
        {
            if (string.IsNullOrEmpty(clientid))
            {
                return BadRequest("Clientid is required.");
            }

            lock (lockObj)
            {
                // Increment the sequence number atomically
                clientSequences.AddOrUpdate(clientid, 1, (key, oldValue) => oldValue + 1);

                // Return the new sequence number
                return Ok(clientSequences[clientid]);
            }
        }
    }
}
