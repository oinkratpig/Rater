using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Rater
{
	/// <summary>
	/// Controls the program.
	/// </summary>
	public partial class Controller : Node
	{
		[Export] private int _defaultMax = 10;
		[ExportGroup("Component")]
		[Export] private Label _addLabel;
		[Export] private VBoxContainer _rateableContainer;
		[Export] private LineEdit _newName;
		[Export] private LineEdit _newValue;
		[Export] private LineEdit _newMax;
		[ExportGroup("Prefabs")]
		[Export] private PackedScene _rateableUI;
		private static List<RateableUI> _rateables;
		private bool _hover;

		// Constructor
		public Controller()
		{
			_rateables = new List<RateableUI>();

		} // end constructor

		public override void _Ready()
		{
			_addLabel.GuiInput += OnAddClicked;
			_addLabel.MouseEntered += () => { _hover = true; };
			_addLabel.MouseExited += () => { _hover = false; };

			ClearAllLineEdits();

		} // end _Ready

		public override void _UnhandledKeyInput(InputEvent @event)
		{
			if (@event.IsActionPressed("quick_save"))
			{
				SaveData.Save(_rateables);
				GetViewport().SetInputAsHandled();
			}
			else if (@event.IsActionPressed("quick_load"))
			{
				foreach (RateableUI rateable in _rateables)
					rateable.QueueFree();
				_rateables.Clear();

				SaveData.Load(_rateableContainer, _rateableUI);
				GetViewport().SetInputAsHandled();
			}

		} // end _UnhandledKeyInput

		public static void CreateRateableUI(VBoxContainer rateableContainer, PackedScene rateableUIPrefab,
			string rateableName, int value, int max)
		{
			RateableUI rateable = rateableUIPrefab.Instantiate<RateableUI>();
			rateableContainer.AddChild(rateable);
			_rateables.Add(rateable);
			rateable.TreeExited += () => {
				_rateables.Remove(rateable);
				UpdateList(rateableContainer);
			};
			rateable.RateableName = rateableName;
			rateable.MaxValue = max;
			rateable.Value = value;

		} // end CreateRateableUI

		public void OnAddClicked(InputEvent @event)
		{
			if (_hover
				&& @event is InputEventMouse mouseEvent
				&& @event.IsPressed()
				&& mouseEvent.ButtonMask == MouseButtonMask.Left)
			{
				// Invalid name
				if (string.IsNullOrWhiteSpace(_newName.Text))
				{
					ClearAllLineEdits();
					return;
				}
				// Invalid value
				int value;
				if (!int.TryParse(_newValue.Text, out value))
				{
					ClearAllLineEdits();
					return;
				}
				// Invalid max
				int max = 0;
				if (!int.TryParse(_newMax.Text, out max) || max <= 0)
				{
					ClearAllLineEdits();
					return;
				}

				// Create new rateable
				_defaultMax = max;
				CreateRateableUI(_rateableContainer, _rateableUI, _newName.Text, value, max);
				ClearAllLineEdits();
				UpdateList(_rateableContainer);
			}

		} // end OnAddClicked

		public static void UpdateList(VBoxContainer rateablesContainer)
		{
			List<RateableUI> sortedRateables = _rateables.OrderByDescending(f => f.Percentage).ToList();

			int i = 0;
			foreach(RateableUI rateable in sortedRateables)
			{
				rateablesContainer.MoveChild(rateable, i);
				rateable.OrderNumber = i + 1;
				i++;
			}

		} // end UpdateList

		public void ClearAllLineEdits()
		{
			_newName.Text = string.Empty;
			_newValue.Text = Mathf.Round(_defaultMax / 2f).ToString();
			_newMax.Text = _defaultMax.ToString();

		} // end ClearAllLineEdits

	} // end class Controller

} // end namespace