namespace Qanat.Common.Util;

public static class DefaultTextHelper
{
    private const string LoremIpsumParagraph =
        "Lorem ipsum dolor sit amet consectetur. Euismod ultricies vel dui faucibus libero vitae nec nec lectus. Lacus suspendisse quis dolor nibh tellus eget. Elementum felis amet neque gravida neque lectus magna tempor vitae. Quam cras aliquet dolor iaculis ultrices justo. Cras luctus at pellentesque eu eu. Magna cras suspendisse elementum id eget aenean. Non in elementum vestibulum pulvinar purus mi. Commodo in risus nisl pharetra diam convallis velit. Ullamcorper mollis adipiscing porttitor convallis viverra quam. Imperdiet urna accumsan nec risus. Orci urna libero sollicitudin amet pellentesque enim amet viverra. Ac nisl ipsum cursus vitae. Quam tincidunt penatibus est laoreet neque sed.";

    public static string GetDefaultTextByNumberOfParagraphsForHtml(int paragraphsCount)
    {
        var defaultText = "";
        for (var i = 0; i < paragraphsCount; i++)
        {
            defaultText += LoremIpsumParagraph + "<br /><br />";
        }

        return defaultText;
    }
}