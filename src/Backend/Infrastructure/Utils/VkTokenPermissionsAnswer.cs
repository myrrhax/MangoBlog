namespace Infrastructure.Utils;

internal class VkTokenPermissionsAnswer
{
    public int Mask { get; set; }
    public IEnumerable<VkPermission> Permissions { get; set; } = [];
}
