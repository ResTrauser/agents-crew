using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AgentsCrew.Core.Knowledge;
using AgentsCrew.Core.Knowledge.ChunkingStrategies;
using Xunit;

namespace AgentsCrew.Tests
{
    public class KnowledgeSourceTests
    {
        [Fact]
        public async Task TextFileKnowledgeSource_WithFixedChunker_ShouldChunkProperly()
        {
            // Arrange
            string filename = "test_knowledge.txt";
            string content = "This is a test document. It has multiple sentences. We will chunk it by fixed size.";
            await File.WriteAllTextAsync(filename, content);

            // Use FixedSizeChunker
            var chunker = new FixedSizeChunker(chunkSize: 20, overlap: 5);
            var source = new TextFileKnowledgeSource(filename, chunker);

            // Act
            var chunksQuery = await source.GetContentChunksAsync();
            var chunks = chunksQuery.ToList();

            // Assert
            Assert.NotNull(chunks);
            Assert.True(chunks.Count > 1);
            Assert.Contains(chunks, c => c.Contains("document"));

            // Cleanup
            if (File.Exists(filename)) File.Delete(filename);
        }

        [Fact]
        public async Task JsonKnowledgeSource_WithSentenceChunker_ShouldExtractValuesAndChunk()
        {
            // Arrange
            string filename = "test_knowledge.json";
            string content = @"{
                ""title"": ""Test Document"",
                ""description"": ""This is a test description. It contains two sentences."",
                ""metadata"": {
                    ""author"": ""Tester""
                }
            }";
            await File.WriteAllTextAsync(filename, content);

            // Use SentenceChunker
            var chunker = new SentenceChunker();
            var source = new JsonKnowledgeSource(filename, chunker);

            // Act
            var chunksQuery = await source.GetContentChunksAsync();
            var chunks = chunksQuery.ToList();

            // Assert
            Assert.NotNull(chunks);
            // It should extract Title, Description, and Author values, then sentence chunk
            // "This is a test description." and "It contains two sentences."
            Assert.True(chunks.Count >= 2);
            Assert.Contains(chunks, c => c.Trim().Contains("This is a test description."));

            // Cleanup
            if (File.Exists(filename)) File.Delete(filename);
        }

        [Fact]
        public async Task XmlKnowledgeSource_ShouldExtractTextNodes()
        {
            // Arrange
            string filename = "test_knowledge.xml";
            string content = @"<?xml version=""1.0""?>
            <document>
                <title>XML Document</title>
                <body>This is the body text. It is very interesting.</body>
            </document>";
            await File.WriteAllTextAsync(filename, content);

            // Default chunker
            var source = new XmlKnowledgeSource(filename);

            // Act
            var chunksQuery = await source.GetContentChunksAsync();
            var chunks = chunksQuery.ToList();

            // Assert
            Assert.NotNull(chunks);
            Assert.True(chunks.Count > 0);
            var merged = string.Join(" ", chunks);
            Assert.Contains("XML Document", merged);
            Assert.Contains("This is the body text", merged);

            // Cleanup
            if (File.Exists(filename)) File.Delete(filename);
        }
    }
}
