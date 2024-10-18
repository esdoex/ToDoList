namespace ToDoList.Helpers
{
    public static class IdEncoderHelper
    {
        public static string EncodeId(int id)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(id.ToString());
            return Convert.ToBase64String(bytes);
        }

        public static int DecodeId(string encodedId)
        {
            var bytes = Convert.FromBase64String(encodedId);
            var decodedString = System.Text.Encoding.UTF8.GetString(bytes);
            return int.Parse(decodedString);
        }
    }
}
