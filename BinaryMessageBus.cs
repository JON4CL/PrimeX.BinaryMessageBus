using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PrimeX.BinaryMessageBus.Parser;

namespace PrimeX.BinaryMessageBus
{
    /// <summary>
    /// 
    /// </summary>
    public static class BinaryMessageBus
    {
        /// <summary>
        /// Diccionario de mensajes segun el tipo de mensaje 
        /// </summary>
        private static Dictionary<byte, BlockingCollection<byte[]>> BlockingCollectionDictionary = new Dictionary<byte, BlockingCollection<byte[]>>();

        /// <summary>
        /// Diccionario de parsers segun el tipo de mensaje 
        /// </summary>
        private static Dictionary<byte, List<IMessageParser>> MessageParserDictionary = new Dictionary<byte, List<IMessageParser>>();

        /// <summary>
        /// Diccionario de hilos que procesaran los mensajes
        /// </summary>
        private static Dictionary<byte, Task> MessageParserTaskDictionary = new Dictionary<byte, Task>();
        
        /// <summary>
        /// Load configuration from a config file and load the parsers in memory
        /// </summary>
        public static void LoadConfig()
        {
            // Load all the classes in one list to process them.
            List<IMessageParser> loadedMessageParser = GetMessageParsers();

            // Filter the parser by MessageType and create a specific list by MessageType
            int i = 0;
            foreach (IMessageParser parserClassInstance in loadedMessageParser)
            {
                List<IMessageParser> messageParserList;

                // Look into the dictionary if exist a list for the specific MessageType
                if (!MessageParserDictionary.TryGetValue(parserClassInstance.GetMessageType(), out messageParserList))
                {
                    // If not exist the MessageParserList create a new one
                    // and add it to the MessageParserDictionary
                    messageParserList = new List<IMessageParser>();
                    MessageParserDictionary.Add(parserClassInstance.GetMessageType(), messageParserList);
                }

                // Add the instance to the MessageParserList
                Console.WriteLine("Added {0} {1} {2}", i++, parserClassInstance.GetMessageType(), parserClassInstance.GetGuid());
                messageParserList.Add(parserClassInstance);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static List<IMessageParser> GetMessageParsers()
        {
            List<IMessageParser> list = new List<IMessageParser>();
            for (int x = 0; x <= 5; x++)
            {
                list.Add(new MessageCommand());
            }
            for (int x = 0; x <= 5; x++)
            {
                list.Add(new MessagePersistence());
            }

            return list;
        }

        /// <summary>
        /// Add message to the bus to be processed
        /// </summary>
        /// <param name="msg"></param>
        public static void AddMessage(byte[] msg)
        {
            // Intenta obtener la BlockingCollection de mensajes segun su tipo
            BlockingCollection<byte[]> messageBlockingCollection;
            if (!BlockingCollectionDictionary.TryGetValue(msg[0], out messageBlockingCollection))
            {
                // Si no hay una BlockingCollection de mensajes la crea
                // y la agrega al diccionario de BlockingCollections
                messageBlockingCollection = new BlockingCollection<byte[]>();
                BlockingCollectionDictionary.Add(msg[0], messageBlockingCollection);

                // Como se creo una nueva BlockingCollectionList
                // se debe crear una nueva tarea para procesar los eventos de esa BlockingCollection
                MessageParserTaskDictionary.Add(msg[0], Task.Run(() =>
                    {
                        byte messageType = msg[0];
                        // Obtiene los mensajes de la BlockingCollectionDictionary 
                        foreach (byte[] binaryMessage in BlockingCollectionDictionary[messageType].GetConsumingEnumerable())
                        {
                            // Clona el mensaje para poder modificarlo si es necesario
                            byte[] binaryMessageTemp = (byte[])binaryMessage.Clone();

                            // Procesa el mensaje en todos los MessageParser
                            foreach (IMessageParser messageParser in MessageParserDictionary[messageType])
                            {
                                // Procesa el mensaje
                                // retorna true si debe seguir procesandose
                                // retorna false si no debe seguir procesandose
                                if (!messageParser.ProcessMessage(binaryMessageTemp, ref binaryMessageTemp))
                                {
                                    // stop processing
                                    break;
                                }
                                else
                                {
                                    // continue processing
                                }
                            }
                        }
                    }
                ));
            }

            // Agrega el mensaje a la BlockingCollection para ser procesado
            messageBlockingCollection.Add(msg);
        }
    }
}
