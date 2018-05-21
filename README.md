# Hsp.Moscow

**M**IDI to **OSC** **O**verpass for **W**indows

Hsp.Moscow is an extensible MIDI vs. OSC bridge (and vice versa). It is very similar to other projects, like OSCII-bot from Cockos (https://www.cockos.com/oscii-bot) but differs in the following points:

1. It only runs on Windows. While the codebase *should* be cross-platform compatible and be able to run on Mono, it is still primarily designed as a Windows service.
2. It is designed to be GUI-less: it runs as a Windows service and therefore provides no user interface, but starts up automatically when your computer starts.
3. It currently only supports one MIDI device and one OSC host.
4. It is extensible with compiled .NET CLR assemblies, opposed to script files (EEL)

## How to install

Install the Windows service by downloading Moscow and then running `hsp.moscow.exe install [serviceName]`. `servicename` is optional.

You will most probably need to be a local administrator to do so.

## How to uninstall

Run `hsp.moscow.exe uninstall` to uninstall the service.

You will most probably need to be a local administrator to do so.

## Configuration

Moscow is configured entirely by modifying it's configuration file `Hsp.Moscow.exe.config`. The following settings are supported:

- `InputMidiDevice`: specifies the MIDI input device to use
- `OscHost`: specifies the OSC host to use. This should be an IP address
- `OscPortOut`: specifies the UDP port to send OSC messages to
- `OscPortIn`: specifies the UDP port to listen for incoming OSC messages
- `PluginFolder`: specifies the folder where to look for plugins. This is optional. It defaults to `<installLocation>\PlugIns`

## How does it work?

Moscow by itself does not really do anything. All processing logic is done by plugins that hook into Moscow. Moscow provides access to sending and receiving MIDI / OSC messages. There is one sample plugin included, `Hsp.Moscow.TrackMuter`. See below for details.

## `Hsp.Moscow.TrackMuter`?

TrackMuter is a plugin that responds to MIDI program change messages and mutes / unmutes tracks on the OSC host according to it's track names. Every track that has the string `Ch<xx>-PC<yyy>` in its name is affected by this plugin. Where `xx` stands for the MIDI channel and `yyy` for the program change number.

So for example:
If Moscow receives `program change 10` message on `channel 1`, all tracks wich contain `Ch00` in their names are muted, except the tracks which contain `Ch00-PC010`.

## Extensibility?

Moscow is easily extendible by writing a .NET CLR assembly. To do so, just create a new class-library project and add `Hsp.Moscow.Extensibility.dll` as a reference. To enable Moscow to find your extension, place the .dll file into the plugins folder of Moscow.

Moscow, on startup, then loads all .dll files inside the plugins folder and scans the assembly for all classes that implement the interface `IMoscowPlugin`. Check the documentation of that interface to see methods and events provided and what / when they are to be used.

**Note**: In order for Moscow to be able to create an instance of your plugin, the class must provide at least one paramter-less constructor.

## Debugging

You can also Moscow in "debug-mode". To do so, just start it as a normal executable from `cmd.exe` using `hsp.moscow.exe debug`. It will then NOT run as a service but will dump MIDI and OSC messages to stdout.
