using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace VioletGrass.Middleware.Features
{
    public class FeatureCollectionTest
    {
        [Fact]
        public async Task FeatureCollection_Add_ByContextConstructor()
        {
            string value = null;
            // arrange
            var stack = new MiddlewareBuilder<Context>()
                .Use(async (context, next) =>
                {
                    value = context.Features.Get<string>();

                    await next(context);
                })
                .Build();

            // act
            await stack(new Context("Hello"));

            // assert
            Assert.Equal("Hello", value);
        }


        [Fact]
        public async Task FeatureCollection_Get_ByContextExtension()
        {
            string value = null;
            // arrange
            var stack = new MiddlewareBuilder<Context>()
                .Use(async (context, next) =>
                {
                    value = context.Feature<string>();

                    await next(context);
                })
                .Build();

            // act
            await stack(new Context("Hello"));

            // assert
            Assert.Equal("Hello", value);
        }
    }
}