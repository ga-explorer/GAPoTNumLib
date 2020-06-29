using System;
using GAPoTNumLib.GAPoT;
using GAPoTNumLib.Structures;
using GAPoTNumLib.Text.Markdown;

namespace GAPoTNumLib.Samples
{
    public static class ParsingSample3
    {
        public static void Execute()
        {
            //Examples:
            //GAPoT bivector using terms form:
            //  -1.3<>, 1.2<1,2>, -4.6<3,4>

            var sourceText =
                "-1.3<>, 1.2<1,2>, -4.6<3,4>";

            var parsingResults = new IronyParsingResults(
                new GaPoTNumBivectorConstructorGrammar(), 
                sourceText
            );

            var bivector = sourceText.GaPoTNumParseBivector();

            var composer = new MarkdownComposer();

            composer
                .AppendLine(parsingResults.ToString());

            if (!parsingResults.ContainsErrorMessages && bivector != null)
            {
                composer
                    .AppendLine()
                    .AppendLine(bivector.TermsToText())
                    .AppendLine()
                    .AppendLine(bivector.TermsToLaTeX());
            }

            Console.WriteLine(composer.ToString());
        }
    }
}