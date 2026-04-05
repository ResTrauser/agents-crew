using System;
using System.Linq;
using System.Threading.Tasks;
using AgentsCrew.Core.Memory;
using Microsoft.SemanticKernel.Memory;
using Xunit;

namespace AgentsCrew.Tests
{
    public class MemoryContextTests
    {
        [Fact]
        public async Task SessionMemory_ShouldStoreAndRetrieveValues()
        {
            // Arrange
            var sessionId = Guid.NewGuid().ToString();
            var sessionMemory = new InMemorySessionMemory(sessionId);
            
            // Act
            await sessionMemory.SetAsync("user_name", "TestUser");
            await sessionMemory.SetAsync("current_task", "TestingMemory");
            
            var name = await sessionMemory.GetAsync<string>("user_name");
            var task = await sessionMemory.GetAsync<string>("current_task");
            
            // Assert
            Assert.Equal("TestUser", name);
            Assert.Equal("TestingMemory", task);
            Assert.Equal(sessionId, sessionMemory.SessionId);
        }
        
        [Fact]
        public async Task SessionMemory_Remove_ShouldDeleteValue()
        {
            // Arrange
            var sessionMemory = new InMemorySessionMemory("session-1");
            await sessionMemory.SetAsync("temp_key", "temp_value");
            
            // Act
            await sessionMemory.RemoveAsync("temp_key");
            var value = await sessionMemory.GetAsync<string>("temp_key");
            
            // Assert
            Assert.Null(value);
        }
    }
}
