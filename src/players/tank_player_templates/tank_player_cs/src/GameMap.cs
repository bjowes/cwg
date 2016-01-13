/******************************************************************************
*
* Copyright Consoden AB, 2014
*
* Created by: Joel Ottosson / joot
*
******************************************************************************/
using System;

namespace tank_player_cs
{
	//Contains helper methods for reading a GameState and moving around in the game.
	class GameMap
	{
		private int tankId;
		private Consoden.TankGame.GameState gameState;
		private DateTime startOfDay = new DateTime (DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

		//Constructor, creates a new GameMap from a GameState.
		public GameMap (int tankId, Consoden.TankGame.GameState gameState)
		{
			this.tankId=tankId;
			this.gameState=gameState;
		}

		//X size of game field
		public int SizeX {
			get {
				return gameState.Width.Val;
			}
		}

		//Y size of game field
		public int SizeY {
			get {
				return gameState.Height.Val;
			}
		}

		//Own tank position
		public Position OwnPosition {
			get {
				return new Position (gameState.Tanks [tankId].Obj.PosX.Val, gameState.Tanks [tankId].Obj.PosY.Val);
			}
		}

		//Enemy tank position
		public Position EnemyPosition {
			get {
				int i = (tankId + 1) % 2;
				
				if(gameState.Tanks [i].Obj.SmokeLeft.Val > 0){
					Random rnd = new Random();
					return new Position (rnd.Next(0,gameState.Width.Val - 1), rnd.Next(0,gameState.Height.Val -1));

					
				}else{
					return new Position (gameState.Tanks [i].Obj.PosX.Val, gameState.Tanks [i].Obj.PosY.Val);
				}
			}
		}

		//Returns how many lasers the player has
		public int getLaserCount {
			get {
				return gameState.Tanks [tankId].Obj.Lasers.Val;
			}
		}

		//Penguin position
		public Position PenguinPosition {
			get {
				
				return new Position (gameState.TheDude.Obj.PosX.Val, gameState.TheDude.Obj.PosY.Val);
			}
		}

		//Cheks if you have some smoke avaliable
		public bool HasSmoke   {
			get {
				
				return gameState.Tanks [tankId].Obj.HasSmoke.Val;
			}
		}
		
		//Cheks if you have some smoke avaliable
		public bool HasRedeemer   {
			get {
				
				return gameState.Tanks [tankId].Obj.HasRedeemer.Val;
			}
		}

		//Check if square is a wall.
		public bool IsWall (Position p)
		{
			return RawVal (p.X, p.Y) == 'x';
		}

		//Check if square has a smoke grenade.
		public bool IsSmokeGrenade (Position p)
		{
			return RawVal (p.X, p.Y) == 's';
		}
		
		//Check if square has redeemerAmmo.
		public bool isRedeemerAmmo (Position p)
		{
			return RawVal (p.X, p.Y) == 'r';
		}

		//Check if there is a mine in this square.
		public bool IsMine (Position p)
		{
			return RawVal (p.X, p.Y) == 'o';
		}

		//Check if there is a coin in this square.
		public bool IsCoin (Position p)
		{
			return RawVal (p.X, p.Y) == '$';
		}

                //Check if there is poison gas in this square.
		public bool IsPoisonGas (Position p)
		{
			return RawVal (p.X, p.Y) == 'p';
		}

	        //Check if there is poison gas in this square.
		public bool IsLaserAmmo (Position p)
		{
			return RawVal (p.X, p.Y) == 'l';
		}

		/*
			
			 Returns an objet with information about the missile. If enemy_tank is set to false
			 the missile of your own player will be returned otherwise its the enemys missile.
			 
			 If no missile exists the function will still return a missile object but it will be null.
			 To get the other field you need to use getter methods wich are just the name of the field you want to get.
			 
			 Se the game documentation for details about the different fields.
			 
			 example:
			 	consoden.tankgame.Missile missile = gm.GetMissile(false);
			    if(missile != null){
			 	    System.out.println("enemy missile head position is " + missile.headPosX().getVal() + "," + missile.headPosY().getVal() );
			    }
		*/
		public Consoden.TankGame.Missile GetMissile(bool enemy_tank){
			int id = 0;
			if(enemy_tank){
				id = (tankId + 1) % 2;
			}else{
				id = tankId;
			}
			
			for (int i=0; i<this.gameState.Missiles.Count; i++) {
				if (gameState.Missiles [i].IsNull ()){
					continue;
				}
				
				Consoden.TankGame.Missile missile = gameState.Missiles [i].Obj;	
				if(missile.TankId.Val == id){
					return missile;
				}
			}
			
			return null;
		}
		
		//Functionality identical to GetMissile
		public Consoden.TankGame.Redeemer GetRedeemer(bool enemy_tank){
			int id = 0;
			if(enemy_tank){
				id = (tankId + 1) % 2;
			}else{
				id = tankId;
			}
			
			for (int i=0; i<this.gameState.Redeemers.Count; i++) {
				if (gameState.Redeemers [i].IsNull ()){
					continue;
				}
				
				Consoden.TankGame.Redeemer redeemer = gameState.Redeemers [i].Obj;	
				if(redeemer.TankId.Val == id){
					return redeemer;
				}
			}
			
			return null;
		}

		//Is there a missile in this square
		public bool IsMissileInPosition (Position p)
		{
			for (int i=0; i<this.gameState.Missiles.Count; i++) {
				if (gameState.Missiles [i].IsNull ())
					continue;

				Consoden.TankGame.Missile missile = gameState.Missiles [i].Obj;
				if ((p.X == missile.HeadPosX.Val && p.Y == missile.HeadPosY.Val) ||
					(p.X == missile.TailPosX.Val && p.Y == missile.TailPosY.Val)) {
					return true;
				}
			}
			return false;
		}
		
		//Is there a redeemer in this square
		public bool IsRedeemerInPosition (Position p)
		{
			for (int i=0; i<this.gameState.Redeemers.Count; i++) {
				if (gameState.Redeemers [i].IsNull ())
					continue;

				Consoden.TankGame.Redeemer redeemer = gameState.Redeemers [i].Obj;
				if (p.X == redeemer.PosX.Val && p.Y == redeemer.PosY.Val) {
					return true;
				}
			}
			return false;
		}

		//Move p one step in specified direction and returns the new position.
		public Position Move(Position p, Consoden.TankGame.Direction.Enumeration d)
		{
			switch (d) {
			case Consoden.TankGame.Direction.Enumeration.Left:
				return new Position ((p.X - 1 + SizeX) % SizeX, p.Y);
			case Consoden.TankGame.Direction.Enumeration.Right:
				return new Position ((p.X + 1) % SizeX, p.Y);
			case Consoden.TankGame.Direction.Enumeration.Up:
				return new Position (p.X, (p.Y - 1 + SizeY) % SizeY);
			case Consoden.TankGame.Direction.Enumeration.Down:
				return new Position (p.X, (p.Y + 1) % SizeY);
			default:
				return p;
			}
		}

		//Milliseconds left until the joystick will be readout next time.
		public int TimeToNextMove()
		{
			TimeSpan elapsedToday = DateTime.Now - startOfDay;
			return gameState.NextMove.Val - (int)elapsedToday.TotalMilliseconds;
		}
	
		//Print game map
		public void PrintMap ()
		{
			for (int y=0; y<this.SizeY; y++) {
				for (int x=0; x<this.SizeX; x++) {
					int i = ToIndex (x, y);
					Console.Write (Convert.ToChar (gameState.Board.Val [i]));
				}
				Console.WriteLine ();
			}
			Console.WriteLine ();
		}

		//Print game state
		public void PrintState() 
		{
		    Console.WriteLine ("Game board");
		    PrintMap();

		    Console.WriteLine ("Own position {0},{1}", OwnPosition.X, OwnPosition.Y);
		    Console.WriteLine ("Enemy position {0},{1}", EnemyPosition.X, EnemyPosition.Y);

		    Console.WriteLine ("Active missiles");

			for (int i=0; i<this.gameState.Missiles.Count; i++) {
				if (gameState.Missiles [i].IsNull ())
					continue;

				Consoden.TankGame.Missile missile = gameState.Missiles [i].Obj;

				Position head = new Position (missile.HeadPosX.Val, missile.HeadPosY.Val);
				Position tail = new Position (missile.TailPosX.Val, missile.TailPosY.Val);

		        Consoden.TankGame.Direction.Enumeration direction = missile.Direction.Val;

		        Console.WriteLine ("Missile position - head {0},{1}", head.X, head.Y);
		        Console.WriteLine ("Missile position - tail {0},{1}", tail.X, tail.Y);
		        Console.Write ("Missile direction ");

		        switch (direction) {
		        case Consoden.TankGame.Direction.Enumeration.Left:
		            Console.WriteLine ("Left");
		            break;
		        case Consoden.TankGame.Direction.Enumeration.Right:
		            Console.WriteLine ("Right");
		            break;
		        case Consoden.TankGame.Direction.Enumeration.Up:
		            Console.WriteLine ("Up");
		            break;
		        case Consoden.TankGame.Direction.Enumeration.Down:
		            Console.WriteLine ("Down");
		            break;
                        case Consoden.TankGame.Direction.Enumeration.Neutral:
                            Console.WriteLine ("Neutral");
                            break;
		        }
		        Console.WriteLine ();
		    }

		    Console.WriteLine ();
		}

		private int ToIndex (int x, int y)
		{
			return x + y * SizeX;
		}

		private char RawVal(int x, int y)
		{
			int i = ToIndex (x, y);
			return Convert.ToChar (gameState.Board.Val [i]);
		}
	}
}
