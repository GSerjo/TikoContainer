TikoContainer
=============

TikoContainer is a tiny IoC container for MonoTouch with support for property injection.

Currently TikoContainer supports following features:

  * Register type as Singleton
  * Resolve single type
  * Resolve type and inject dependencies
  * Property injection
  * BuildUp existing instance (inject dependencies)
  * Clear IoC container

Using TikoContainer
-------------------

### Register type ###


Register type thru Register<TFrom, TTo>() or Register<T>() methods. All type will be registered as Singleton.

	public void RegisterDal()
	{
		TikoContainer.Register<IUserAccountRepository, UserAccountRepository>();
		TikoContainer.Register<PasswordManager>();
	}

### Resolve by type ###
	
Resolve object by type. _userAccountRepository will be initialised with an instance of UserAccountRepository.

	var _userAccountRepository = TikoContainer.Resolve<IUserAccountRepository>();
	

### Resolve with property injection ###

Resolve dependency thru property injection. UserAccountRepository and PasswordManager properties will be injected after call 			

	TikoContainer.Resolve<UserAccountManager>() 
	
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
	
### Resolve dependencies of an exsisting object ###

UserAccountRepository and PasswordManager properties will be injected after BuildUp call.

	var _userAccountManager = new UserAccountManager();
	TikoContainer.BuildUp(_userAccountManager);
	
### Clear TikoContainer ###

Clear IoC container cache, all registered types will be removed.

	TikoContainer.Clear();





  