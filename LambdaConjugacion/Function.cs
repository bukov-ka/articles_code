using System;
using System.Collections.Generic;
using System.Net;
using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Amazon.Lambda.Core;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.LambdaJsonSerializer))]

namespace LambdaConjugacion
{
    public class Function
    {
        const string filterParameterKey = "input";
        public APIGatewayProxyResponse Get(
        APIGatewayProxyRequest request, ILambdaContext context)
        {
            string filter = null;
            if (request.QueryStringParameters.ContainsKey(filterParameterKey))
            {
                filter = request.QueryStringParameters[filterParameterKey];

            }
            var result = this.GetTenVerbs(filter);

            return CreateResponse(result);
        }

        APIGatewayProxyResponse CreateResponse(List<WordViewModel> result)
        {
            int statusCode = (result != null) ?
                (int)HttpStatusCode.OK :
                (int)HttpStatusCode.InternalServerError;

            string body = (result != null) ?
                JsonConvert.SerializeObject(result) : string.Empty;

            var response = new APIGatewayProxyResponse
            {
                StatusCode = statusCode,
                Body = body,
                Headers = new Dictionary<string, string>
        {
            { "Content-Type", "application/json" },
            { "Access-Control-Allow-Origin", "*" }
        }
            };

            return response;
        }


        /// <summary>
        /// Return an object.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
public object FunctionHandler(string filter, ILambdaContext context)
{
    return Data.TenRandomVerbs(string.IsNullOrEmpty(filter) ? null : filter.Split(","));
}
        public List<WordViewModel> GetTenVerbs(string filter)
        {            
            return 
        }
    }
}
