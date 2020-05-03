# FileCabinet
This product has two independent modules:
 - FileCabinetApp - Provides opportunities to work with records.
 - FileCabinetGenerator - Generates records to work in FileCabinetApp.
 **To work with any of modules you need to place configuration files (appsettings.json, constantStrings.json, validation-rules.json) in folder with executable file of module.**
 In order to start app modules you can use command line parameters.
## Available command line parameters for FileCabinetGenerator:
 - --output-type, -t - sets format type to export generated records. Available values: xml, csv
 - --output, -o - sets destination file path
 - --records-amount, -a - sets amount of records to generate
 - --start-id, -i - sets start id to generate records
## Available command line parameters for FileCabinetApp:
 - --validation-rules, -v - sets type of validation rules to use from validation-rules.json file. Available values: default, custom
 - --storage, -s - sets type of storage to use. Available values: memory, filesystem
 - --use-logger - sets logger to log operations while executing
 - --use-stopwatch - sets stopwatch to measure time of methods execution
 Inner commands information you can get using "help" command in application.