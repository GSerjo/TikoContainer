TikoContainer
=============

TikoContainer is a tiny IoC container for MonoTouch with support for property injection.

Currently TikoContainer supports following features:

  * Register type as Singleton
  * Resolve single type
  * Resolve type and inject dependencies
  * Property injection
  * BuildUp existing instance (inject dependencies)

Using TikContainer
==================

Register type thru Register<TFrom, TTo>() or Register<T>() methods. All type will be registered as Singleton.

	public void RegisterDal()
	{
		TikoContainer.Register<IUserAccountRepository, UserAccountRepository>();
		TikoContainer.Register<PasswordManager>();
	}
	
Resolve object by type. _userAccountRepository will be initialised with an instance of UserAccountRepository.

	var _userAccountRepository = TikoContainer.Resolve<IUserAccountRepository>();

Resolve dependency thru property injection. UserAccountRepository and PasswordManager properties will be injected after call TikoContainer.Resolve<UserAccountManager>() 
	
    public class UserAccountManager
    {
        private static readonly Lazy<UserAccountManager> _instance = 
            new Lazy<UserAccountManager>(TikoContainer.Resolve<UserAccountManager>);

        public static UserAccountManager Value
        {
            get { return _instance.Value; }
        }

        [Dependency]
        public IUserAccountRepository UserAccountRepository { get; set; }

        [Dependency]
        public PasswordManager PasswordManager { get; set; }
    }
	



  