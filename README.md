# IA-ProyectoFinal
## Autores
- Sara Isabel Garcia Moral (sarais02). 
- Javier Comas de Frutos (javixxu). Encargado de la herramienta de creacion de mundos procedurales y objectos colocados proceduralmente a lo largo del mapa
## Propuesta

### Herramienta de Creacion de Mundos Procedurales
La creacion de un mundo procedural completamente parametrizada con sistema de capas de terreno y creacion de objectos a lo largo de este.Ademas dicho terreno
generado ha de estar optimizado para que se pueda llegar a generar una gram superfice de terreno.

Como la creacion de un mundo completamente "aleatorio" puede resultar algo compleja se ha decidido establecer los siguientes parametros para que el usuario
pueda modificar a su antojo para crear el mundo con el terreno y objectos que desee):
  - int mapSize: Tamaño del Mapa
  - float NoiseScale: El factor de escala del ruido generado. Un valor mayor producirá un ruido con detalles más finos
  - int Octaves:  El número de octavas utilizadas en el algoritmo de ruido.Cada octava es una capa de ruido que se suma al resultado final.
    A medida que se agregan más octavas, el ruido generado se vuelve más detallado
  - float Persistance: Controla la amplitud de cada octava.Un valor más bajo reducirá el efecto de las octavas posteriores de las octavas posteriores
  - float Lacunarity: Un multiplicador que determina qué tan rápido aumenta la frecuencia para cada octava sucesiva en una función de ruido de Perlin
  - int Seed: Numero aleatorio utilizado para generar el ruido
  - Vector2 Offset: La posición inicial del ruido generado
  - TerrainType[]regions: Layers del terreno que se pueden generar
  - ObjectInMap[]objects: Objectos que se pueden generar a lo largo del mapa
  - bool useFallOff: Generar un suavizado en el ruido generado de tal forma que el mapa tenga forma de isla
  - bool autoUpdate: Cuando se realize un cambio des de el editor, auto actualizar el mapa
  - bool autoRegenerate: Cuando se inicilize este componente autoregenerar el terreno


Establecidos todos estos parametros tendremos varias posibilidades, tipos de configuracion para la generacion. Es posible que nosotros queramos previsualizar el mapa que estemos generando en 2D antes que 3D para ello,
tendremos varios modos de creacion:
  - NoiseMap: Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 2D)
  - ColorMap: Generacion de un Mapa de Ruido con  los bordes del terreno suavizados(Solo visual 2D)
  - FallOff: Generacion de un Mapa de Con los layers de terreno establecidos(Solo visual 3D)
  - NoObjects: Generacion de un Mapa de Con los layers de terreno establecidos y los Objectos puestos para generar(Solo visual 3D)
  - Objects: Generacion de un Mapa de Con los layers de terreno establecidos y los Objectos puestos para generar(Solo visual 3D)
  - NobjectsWithDisplay: Configuracion de ColorMap y NoObjects     
  - All: Configuracion de ColorMap y Objects
### IA de comportamientos de personajes
### Combinacion de ambas propuestas
Sera un nivel en el que la IA se instanciaran proceduralmente por el mapa procedural y realizara sus distintos comportamientos. Pudiendo el jugador observarles desde la lejania ya que si te ven iran a por el.
## Diseño de la solución

### Herramienta de Creacion de Mundos Procedurales
La Herramienta posee varios "elementos basicos" que facilitan la creacion del mundo:
  Class Chunk: Un chunk es una porcion 50x50 del mapa generado, esto se debe a que unity no deja crear mas de 65,534 por ello es necesario dividir la malla del mapa en otras mas pequeñas que que todas estas juntas         conformen el mapa. De esta forma cada chunk es una porcion del mapa generado de 50*50. 
  
      - Un GameObject Padre con el nombre de Chunk+ posMap que contiene a la malla del suelo, bordes y un gameobject que contiene todos los objectos generados en ese chunk.
      -PosMap representa la posicion del chunk en el mapa 



La Herramienta constara de Varios Generadores:

  - Noise: Es el generador de Ruidos de Perlin.Constara de dos Grandes metodos: 
  
      - GenerateNoiseMap(int size,int seed,float noiseScale,int octaves, float persistance, float lacunarity,Vector2 offset) que generara un ruido a partir de los parametros de entrada, 
      devolviendo una matriz de floats con el valor de ruido generado en cada casilla.
      
      - GenerateFalloffMap(int size): Genera un mapa de fallOff para suavizar los bordes del terreno usando esta ecuacion para ello 
  ![image](https://github.com/IAV23-G07/IAV23-G07-ProyectoFinal/assets/82498887/efada9ad-700b-494a-8676-2d9b27243b7f) 

        Siendo b=2.2 y a=3.De esta forma simulamos una especie de isla combinando la matriz que obtenemos del falloff y la matriz de ruido obtenida en el metodo GenerateNoiseMap
  - TextureGenerator: Es una clase para la creacion de una textura, que servira para la representacion del mapa generado. Constara de dos metodos:
       - TextureFromColorMap(Color[] colorMap,int width,int height): crea una textura y le aplica a cada texel de la texura el valor del array color, creando asi una textura a color.
       - TextureFromNoiseMap(float[,] noiseMap): a su misma vez que el metodo anteior este crea una textura para la representacion del mapa creado pero esta vez en blanco y negro interpolando dichos colores con el               noiseMap que contiene el noise de cada celda
  - MeshGenerator: Es el generador de la malla en 3D, como queremos que la malla que estemos creado sea lo mas eficiente posible solo crearemos las caras visibles, pues el mapa generado esta pensado para que no se le          puedan quitar bloques,trozos, etc. Para ello se ha divido en dos partes, el suelo y los bordes.En ambos hay que definir los vertices, triangulos y coordenadas de textura. 
        -
## Controles
El movimiento es un tipico 3ºpersona, en el que el jugador se mueve con las teclas AWSD y rota la camara con el raton.
## Producción

Las tareas se han realizado y el esfuerzo ha sido repartido entre los autores.

| Estado  |  Tarea  |  Fecha  |  Autores |
|:-:|:--|:-:|:-:|


## Referencias

Los recursos de terceros utilizados son de uso público.

- *AI for Games*, Ian Millington
- API Unity
