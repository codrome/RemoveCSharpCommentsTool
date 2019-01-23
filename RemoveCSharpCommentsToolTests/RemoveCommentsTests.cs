using System;
using Xunit;
using RemoveCSharpCommentsTool;
using System.Text;

namespace RemoveCSharpCommentsToolTests
{
    public class RemoveCommentsTests
    {
        [Theory]
        [InlineData("codecode")]
        [InlineData("codecode//comment")]
        [InlineData("code/*comment*/code")]
        [InlineData(@"code/*comment
comment*/code")]
        public void RemoveCommentsSimpleTest(string inputString)
        {
            var sb = new StringBuilder(inputString);
            Program.RemoveComments(sb);
            Assert.Equal("codecode", sb.ToString());
        }

        [Theory]
        [InlineData(@"codecode//comment
///**/comment
/*comment//*///comment
/*comment
comment*/
code//")]
        [InlineData(@"code/**/code
/*/*comment*/
code//comment")]
        public void RemoveCommentsExtendedTest(string inputString)
        {
            var sb = new StringBuilder(inputString);
            while (Program.RemoveComments(sb)) { }
            Assert.Equal("codecode\r\ncode", sb.ToString());
        }

        [Theory]
        [InlineData(@"/*commnent*/")]
        [InlineData(@"/*commnent
comment*/")]
        [InlineData(@"//comment
//comment
/*comment*/")]
        public void RemoveCommentsEmptyTest(string inputString)
        {
            var sb = new StringBuilder(inputString);
            while (Program.RemoveComments(sb)) { }
            Assert.Equal(string.Empty, sb.ToString());
        }

        [Fact]
        public void RemoveCommentsSpecTest1()
        {
            var inputString = @"code
	/*comments */
	/*comments
	 comments*/
    code";
            var sb = new StringBuilder(inputString);
            while (Program.RemoveComments(sb)) { }
            Assert.Equal(@"code
	
	
    code", sb.ToString());
        }
    }

}
