using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Reactive;
using System.Reactive.Linq;

namespace IsLoadingReactiveUI.ViewModels;

public record Form
{
    public bool Throw { get; set; } = false;
}

public class MyViewModel : ReactiveObject
{
    [ObservableAsProperty]
    public Form? Form { get; }

    [ObservableAsProperty]
    public bool IsLoading { get; }

    [Reactive]
    public Exception? Exception { get; private set; } = null;

    [Reactive]
    public bool Trigger { get; set; } = false;

    public MyViewModel()
    {
        Submit = ReactiveCommand.CreateFromTask<Form>(async form =>
        {
            Exception = null;
            Trigger = false;

            await Task.Delay(2000);

            if (form.Throw)
            {
                throw new ArgumentException();
            }
        });

        Submit.IsExecuting.ToPropertyEx(this, x => x.IsLoading, scheduler: RxApp.MainThreadScheduler);
        Submit.ThrownExceptions.BindTo(this, x => x.Exception);

        this.WhenAnyValue(x => x.Trigger)
            .Skip(1)
            .Select(x => new Form { Throw = true })
            .InvokeCommand(Submit);
    }

    public ReactiveCommand<Form, Unit> Submit { get; }
}