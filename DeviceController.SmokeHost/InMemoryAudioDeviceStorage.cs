using System.Collections.Concurrent;
using DeviceControllerLib.Models.RestApi;
using DeviceControllerLib.Services;

namespace DeviceController.SmokeHost;

public sealed class InMemoryAudioDeviceStorage : IAudioDeviceStorage
{
    private readonly ConcurrentDictionary<(string PnpId, string HostName), EntireDeviceMessage> _devices = new();

    public IEnumerable<EntireDeviceMessage> GetAll() => [.. _devices.Values];

    public void Add(EntireDeviceMessage entireDeviceMessage)
    {
        _devices[(entireDeviceMessage.PnpId, entireDeviceMessage.HostName)] = entireDeviceMessage;
    }

    public void Remove(string pnpId, string hostName)
    {
        _ = _devices.TryRemove((pnpId, hostName), out _);
    }

    public void UpdateVolume(string pnpId, string hostName, VolumeChangeMessage volumeChangeMessage)
    {
        if (!_devices.TryGetValue((pnpId, hostName), out var existing))
        {
            return;
        }

        var updated = volumeChangeMessage.DeviceMessageType == DeviceMessageType.VolumeRenderChanged
            ? existing with
            {
                RenderVolume = volumeChangeMessage.Volume,
                UpdateDate = volumeChangeMessage.UpdateDate,
                DeviceMessageType = volumeChangeMessage.DeviceMessageType
            }
            : existing with
            {
                CaptureVolume = volumeChangeMessage.Volume,
                UpdateDate = volumeChangeMessage.UpdateDate,
                DeviceMessageType = volumeChangeMessage.DeviceMessageType
            };

        _devices[(pnpId, hostName)] = updated;
    }

    public IEnumerable<EntireDeviceMessage> Search(string query)
    {
        var lowered = query.ToLowerInvariant();

        return
        [
            .. _devices.Values
                .Where(d =>
                    d.PnpId.ToLowerInvariant().Contains(lowered) ||
                    d.Name.ToLowerInvariant().Contains(lowered) ||
                    d.HostName.ToLowerInvariant().Contains(lowered) ||
                    d.OperationSystemName.ToLowerInvariant().Contains(lowered))
        ];
    }
}
