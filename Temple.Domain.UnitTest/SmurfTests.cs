using Temple.Domain.Entities.Smurfs;

namespace Temple.Domain.UnitTest;

public class SmurfTests
{
    [Fact]
    public void NewSmurfHasAnUnspecifiedRole_AndPreservesExistingConstructorBehavior()
    {
        var smurf = new Smurf();

        Assert.Equal(SmurfRole.Unspecified, smurf.Role);
        Assert.Equal("", smurf.Name);
        Assert.Equal("", smurf.ToString());
    }

    [Theory]
    [InlineData(SmurfRole.Unspecified)]
    [InlineData(SmurfRole.Builder)]
    [InlineData(SmurfRole.Farmer)]
    public void RoleDoesNotChangeTheSmurfsDisplayName(SmurfRole role)
    {
        var smurf = new Smurf { Name = "Smolfine", Role = role };

        Assert.Equal("Smolfine", smurf.ToString());
    }
}
