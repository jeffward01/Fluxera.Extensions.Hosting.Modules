﻿namespace WebApplication1.Model
{
	using Fluxera.Entity;

	public class Customer : AggregateRoot<Customer, string>
	{
		public string Name { get; set; }
	}
}
