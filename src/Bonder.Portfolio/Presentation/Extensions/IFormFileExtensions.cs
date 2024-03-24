using Microsoft.AspNetCore.Http;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Extensions;
public static class IFormFileExtensions
{
    public static List<Stream> OpenReadStreams(this IFormFileCollection files)
    {
        return files.Select(file => file.OpenReadStream()).ToList();
    }

    public static void Dispose(this List<Stream> streams)
    {
        foreach(var stream in streams)
        { 
            stream.Dispose(); 
        }
    }
}
