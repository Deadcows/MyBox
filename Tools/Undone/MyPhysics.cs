using UnityEngine;

namespace MyBox
{
	public class MyPhysics
	{
		public static readonly RaycastHit2D[] Hits = new RaycastHit2D[20];
		public static readonly Collider2D[] Colliders = new Collider2D[20];


		public static int CircleOverlap2D(Vector2 pos, float radius, int mask = 1 << 0, float min = float.NegativeInfinity,
			float max = float.PositiveInfinity)
		{
			return Physics2D.OverlapCircleNonAlloc(pos, radius, Colliders, mask, min, max);
		}

		public static int Raycast2D(Vector2 position, Vector2 direction)
		{
			return Physics2D.RaycastNonAlloc(position, direction, Hits);
		}

		public static int Raycast2D(Vector2 position, Vector2 direction, float distance)
		{
			return Physics2D.RaycastNonAlloc(position, direction, Hits, distance);
		}

		public static int Raycast2D(Vector2 position, Vector2 direction, float distance, int mask)
		{
			return Physics2D.RaycastNonAlloc(position, direction, Hits, distance, mask);
		}

		public static int Raycast2D(Vector2 position, Vector2 direction, float distance, int mask, float min = float.NegativeInfinity,
			float max = float.PositiveInfinity)
		{
			return Physics2D.RaycastNonAlloc(position, direction, Hits, distance, mask, min, max);
		}


//		void Update()
//		{
//			// считаем кадры  
//			var frames = UnityEngine.Time.frameCount;
//			// каждые 5 кадров делаем что-то, в нашем случае проверяем столкновения.
//			if (frames % 5 == 0)
//			{
//				// мы отпарвляем луч из позиции объекта вправо и хотим получить все столкновения на дистанции в 1 единицу измерения.
//				// эти столкновения будут храниться в Phys.hits а метод вернет нам кол-во этих столкновений. Оно никогда не превысит размер массива Phys.hits.
//				var amount = Phys.Raycast2D(transform.position, Vector2.Right, 1.0f);
//				for (int i = 0; i < amount; i++)
//				{
//					// перебираем все RaycastHit2d попавшие в область рейкаста.
//					var hit = Phis.hits[i];
//				}
//			}
//		}
	}
}