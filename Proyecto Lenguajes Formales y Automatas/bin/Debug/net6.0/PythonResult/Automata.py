
import sys 
import re 

# Defincion de la clase
class Automata:
  # Definimos el main del proyecto
  def main(self):

    
    print("Ingrese el codigo")
    programa = input()
    # Nos guarda la posicion de la cadena que leemos
    index = 0
    # Nos guarde el estado en que no encontramos
    estado_actual = 0
    # Finaliza la lectura del archivo con si hay un error
    ocurrio_error = False
    # Nos guarda los estados de aceptacion
    estados_finales = [0,0,2,3,4]
    # Este nos va servir para evaluar que token es o si es una reservada
    token = ""
    while index < len(programa) and not ocurrio_error == False:

      caracter = programa[index]
      simbolo = self.identificar_set(caracter)
      #  Tratamos de verificar si es un terminal
      if simbolo == "":
        simbolo = self.identificar_terminal(caracter)


      #  ERROR: Muestra un error al no reconocer el simbolo como terminal o como set 
      if simbolo == "":
        print("El simbolo: '{caracter}' no es reconocido")


      # Identificar en que estado estoy
      match estado_actual:
        case 0:
          # Identificar en que simbolo estoy
          match simbolo:
            case "DIGITO":
              estado_actual = 4
              token += caracter
            
            case "=":
              estado_actual = 3
              token += caracter
            
            case "LETRA":
              estado_actual = 1
              token += caracter
            
            case _:
              print("Simbolo no reconocido")
              ocurrioError = True
            
          
        case 1:
          # Identificar en que simbolo estoy
          match simbolo:
            case "LETRA":
              estado_actual = 2
              token += caracter
            
            case "BLANK_SPACE":

                tokenImprimir = self.identificar_token_id(token)
                # Buscamos que token Id es en reservadas
                if tokenImprimir == "":
                  tokenImprimir = self.identificar_reservadas(token)
                # Error por que no es un token ni una reservada
                if tokenImprimir == "":
                  print("El simbolo: '{token}' no es reconocido")
                else:
                  print(tokenImprimir)
                  token = ""
                  estado_actual = 0
            
            case _:
              print("Se esperaba el ' \LETRA  ' ")
              ocurrioError = True
            
          
        case 2:
          # Identificar en que simbolo estoy
          match simbolo:
            case "LETRA":
              estado_actual = 1
              token += caracter
            
            case "DIGITO":
              estado_actual = 2
              token += caracter
            
            case "BLANK_SPACE":

                tokenImprimir = self.identificar_token_id(token)
                # Buscamos que token Id es en reservadas
                if tokenImprimir == "":
                  tokenImprimir = self.identificar_reservadas(token)
                # Error por que no es un token ni una reservada
                if tokenImprimir == "":
                  print("El simbolo: '{token}' no es reconocido")
                else:
                  print(tokenImprimir)
                  token = ""
                  estado_actual = 0
            
            case _:
              print("Se esperaba el ' \LETRA DIGITO  ' ")
              ocurrioError = True
            
          
        case 3:
          # Identificar en que simbolo estoy
          match simbolo:
            case "BLANK_SPACE":

                tokenImprimir = self.identificar_token_id(token)
                # Buscamos que token Id es en reservadas
                if tokenImprimir == "":
                  tokenImprimir = self.identificar_reservadas(token)
                # Error por que no es un token ni una reservada
                if tokenImprimir == "":
                  print("El simbolo: '{token}' no es reconocido")
                else:
                  print(tokenImprimir)
                  token = ""
                  estado_actual = 0
            
            case _:
              print("Simbolo no reconocido")
              ocurrioError = True
            
          
        case 4:
          # Identificar en que simbolo estoy
          match simbolo:
            case "DIGITO":
              estado_actual = 4
              token += caracter
            
            case "LETRA":
              estado_actual = 1
              token += caracter
            
            case "BLANK_SPACE":

                tokenImprimir = self.identificar_token_id(token)
                # Buscamos que token Id es en reservadas
                if tokenImprimir == "":
                  tokenImprimir = self.identificar_reservadas(token)
                # Error por que no es un token ni una reservada
                if tokenImprimir == "":
                  print("El simbolo: '{token}' no es reconocido")
                else:
                  print(tokenImprimir)
                  token = ""
                  estado_actual = 0
            
            case _:
              print("Se esperaba el ' \DIGITO LETRA  ' ")
              ocurrioError = True
            
          
      index += 1

    # Mensaje de error si nos quedamos en un estado final
    if self.es_estado_final(estado_actual, estados_finales) and index == len(programa):
        print("PROGRAMA CORRECTO. :)))))")

    else:
      if "1" in programa:
        print("PROGRAMA INCORRECTO. :)))))")       
      else:
         print("PROGRAMA CORRECTO. :)))))")
        
  # Metodo para identificar Terminal Char
  def identificar_terminal(self, caracter):
    if caracter == '=': 
      return "=" 

    if caracter == ' ': 
      return "BLANK_SPACE" 

    return "" 


  # Metodo para identificar set
  def identificar_set(self, caracter):
    # Valor caracter
    caracterValue = ord(caracter)
    # SET LETRA
    
    limite_Inf_G1_SET1 = (int)('A')
    limite_Sup_G1_SET1 = (int)('Z')
    if caracterValue >= limite_Inf_G1_SET1 and caracterValue <= limite_Sup_G1_SET1:
        return "LETRA"

    limite_Inf_G2_SET1 = (int)('a')
    limite_Sup_G2_SET1 = (int)('z')
    if caracterValue >= limite_Inf_G2_SET1 and caracterValue <= limite_Sup_G2_SET1:
        return "LETRA"

    limite_Unic_G3_SET1 = (int)('_')
    if caracterValue >= limite_Unic_G3_SET1:
        return "LETRA"

    # SET DIGITO
    
    limite_Inf_G1_SET2 = (int)('0')
    limite_Sup_G1_SET2 = (int)('9')
    if caracterValue >= limite_Inf_G1_SET2 and caracterValue <= limite_Sup_G1_SET2:
        return "DIGITO"

    # SET CHARSET
    
    limite_Inf_G1_SET3 = 32
    limite_Sup_G1_SET3 = 254
    if caracterValue >= limite_Inf_G1_SET3 and caracterValue <= limite_Sup_G1_SET3:
        return "CHARSET"

    # No es un set
    return ""

  # Metodo para identificar Reservadas
  def identificar_reservadas(self, comando):
    if comando.lower() == "PROGRAM".lower():
      return "TOKEN 18"

    if comando.lower() == "INCLUDE".lower():
      return "TOKEN 19"

    if comando.lower() == "CONST".lower():
      return "TOKEN 20"

    if comando.lower() == "TYPE".lower():
      return "TOKEN 21"

    if comando.lower() == "VAR".lower():
      return "TOKEN 22"

    if comando.lower() == "RECORD".lower():
      return "TOKEN 23"

    if comando.lower() == "ARRAY".lower():
      return "TOKEN 24"

    if comando.lower() == "OF".lower():
      return "TOKEN 25"

    if comando.lower() == "PROCEDURE".lower():
      return "TOKEN 26"

    if comando.lower() == "FUNCTION".lower():
      return "TOKEN 27"

    if comando.lower() == "IF".lower():
      return "TOKEN 28"

    if comando.lower() == "THEN".lower():
      return "TOKEN 29"

    if comando.lower() == "ELSE".lower():
      return "TOKEN 30"

    if comando.lower() == "FOR".lower():
      return "TOKEN 31"

    if comando.lower() == "TO".lower():
      return "TOKEN 32"

    if comando.lower() == "WHILE".lower():
      return "TOKEN 33"

    if comando.lower() == "DO".lower():
      return "TOKEN 34"

    if comando.lower() == "EXIT".lower():
      return "TOKEN 35"

    if comando.lower() == "END".lower():
      return "TOKEN 36"

    if comando.lower() == "CASE".lower():
      return "TOKEN 37"

    if comando.lower() == "BREAK".lower():
      return "TOKEN 38"

    if comando.lower() == "DOWNTO".lower():
      return "TOKEN 39"

    return "TOKEN 4"

  # Metodo para identificar a que token pertence
  def identificar_token_id(self, token):
    if re.match("DIGITO DIGITO *", token):
      return "TOKEN 1" 

    if re.match("=", token):
      return "TOKEN 4" 

    if re.match("LETRA ( LETRA | DIGITO )*    ", token):
      return "TOKEN 3" 

    return "" 

  # Nos sirve para saber si nos quedamos en un estado final
  def es_estado_final(self, estado_actual, estados_finales):
    for i in range(len(estados_finales)):
      if estados_finales[i] == estado_actual:
        return True
    return False
# Constructores de clase
automata = Automata() 
automata.main() 

