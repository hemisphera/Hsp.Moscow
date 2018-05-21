# Hsp.Moscow

Hsp.Moscow is an extensible MIDI vs. OSC bridge (and vice versa). It is very similar to other projects, like OSCII-bot from Cockos (https://www.cockos.com/oscii-bot) but differs in the following points:

1. It only runs on Windows. While the codebase *should* be cross-platform compatible and be able to run on Mono, it is still primarily designed as a Windows service.
2. It is designed to be GUI-less: it runs as a Windows service and therefore provides no user interface, but starts up automatically when your computer starts.
3. It currently only supports one MIDI device and one OSC host.
4. It is extensible with compiled .NET CLR assemblies, opposed to script files (EEL)

## How to install

Install the Windows service by downloading Moscow and then running `hsp.moscow.exe install [serviceName]`. Service name is optional.

You will most probably need to be a local administrator to do so.

## How to uninstall

Run `hsp.moscow.exe uninstall` to uninstall the service.

You will most probably need to be a local administrator to do so.

## Configuration

Moscow is configured entirely by modifying it's configuration file `Hsp.Moscow.exe.config`. The following settings are supported:

- `InputMidiDevice`: specifies to MIDI input device to use
- `OscHost`: specifies the OSC host to use. This should be an IP address
- `OscPortOut`: specifies the UDP port to send OSC messages to
- `OscPortIn`: specifies the UDP port to listen for incoming OSC messages
- `PluginFolder`: specifies the folder where to look for plugins. This is optional. It defaults to `<installLocation>\PlugIns`
