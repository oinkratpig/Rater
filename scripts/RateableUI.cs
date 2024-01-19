using Godot;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Rater
{
	/// <summary>
	/// A Rateable represented in the UI.
	/// </summary>
	public partial class RateableUI : Node
	{
		[ExportGroup("Components")]
		[Export] private Label _orderNumber;
		[Export] private ProgressBar _progressBar;
		[Export] private Label _rateableName;
		[Export] private Label _rankValue;
		[Export] private Label _rankMax;
		[Export] private Label _delete;
		private int _value;
		private int _maxValue;
		private bool _hover;

		/// <summary>
		/// The name of this rateable.
		/// </summary>
		public string RateableName {
			get { return _rateableName.Text; }
			set { _rateableName.Text = value; }
		}

		/// <summary>
		/// The number representing the order in the list.
		/// </summary>
		public int OrderNumber {
			get { return _orderNumber.Text.ToInt(); }
			set { _orderNumber.Text = value.ToString(); }
		}

		/// <summary>
		/// Current ranking out of the max value.<br/>
		/// For example, if rated 6/10, <see cref="Value"/> would be 6.
		/// </summary>
		public int Value {
			get { return _value; }
			set {
				_value = value;
				_progressBar.Value = Mathf.Round(Percentage * 100f);
				_rankValue.Text = _value.ToString();
			}
		}

		/// <summary>
		/// Current max value to rank out of.<br/>
		/// For example, if rated 6/10, <see cref="MaxValue"/> would be 10.
		/// </summary>
		public int MaxValue {
			get { return _maxValue; }
			set {
				_maxValue = value;
				_progressBar.Value = Percentage;
				_rankMax.Text = _maxValue.ToString();
			}
		}

		/// <summary>
		/// Percentage used for the <see cref="ProgressBar"/>.
		/// </summary>
		public float Percentage {
			get { return (float) _value / _maxValue; }
		}

		public override void _Ready()
		{
			_delete.GuiInput += OnDeleteGuiInput;
			_delete.MouseEntered += () => { _hover = true; };
			_delete.MouseExited += () => { _hover = false; };

			_value = 0;
			_maxValue = 0;

		} // end _Ready

		public void OnDeleteGuiInput(InputEvent @event)
		{
			if (_hover
				&& @event is InputEventMouse mouseEvent
				&& @event.IsPressed()
				&& mouseEvent.ButtonMask == MouseButtonMask.Left)
			{
				QueueFree();
			}

		} // end OnDeleteClick

	} // end class RateableUI

} // end namespace