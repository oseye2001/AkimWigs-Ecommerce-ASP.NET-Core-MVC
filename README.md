🛍️ AkimWigs – Boutique de Wigs (ASP.NET Core MVC)

AkimWigs est une mini-boutique e-commerce développée en ASP.NET Core MVC 8, avec gestion de produits, panier, checkout, Stripe, et un panneau d'administration complet pour la gestion des wigs (ajout / édition / suppression).

Ce projet a été conçu comme un site vitrine professionnel avec fonctionnalités e-commerce, incluant un design moderne, responsive, et un back-office complet.

✨ Fonctionnalités principales
🎨 Frontend / Boutique

Page d'accueil avec hero visuel + carrousel

Catalogue complet des wigs (images, prix, categories)

Filtrage par catégorie et recherche

Page détail produit (prix, longueur, texture, couleur)

Ajout au panier

Panier (quantité, suppression, total)

Images hébergées dans wwwroot/images/wigs/

SEO optimisé (Title, Meta description, OpenGraph)

Favicon personnalisé

🧾 Checkout

Choix livraison Standard / Express

Saisie des coordonnées client

Résumé de commande

Paiement sécurisé Stripe (API Keys)

Messages de validation

Session persistante

🔐 Admin Panel

(Admin pro pour gérer la boutique)

Login admin sécurisé

Gestion des produits :

Ajouter une wig

Modifier une wig

Supprimer une wig

Recherche + filtrage par catégorie

Interface moderne (couleurs AkimWigs)

🗃️ Base de données

Seed automatique :

3 catégories : Lace Front, Closure, Frontal

6 wigs pré-insérées

SQL Server

EF Core 8 + migrations

🛠️ Technologies utilisées

ASP.NET Core MVC 8

Entity Framework Core 8

SQL Server

Bootstrap 5

Session Cart

Stripe API

Razor Views

LINQ + Async

C# 12

CSS custom (site.css)

📂 Structure du projet
AkimWigs/
│
├── AkimWigs.Core/            # Modèles (Product, Category)
├── AkimWigs.Infrastructure/  # DbContext, Data Seed
├── AkimWigs.Web/             # MVC (Controllers, Views, wwwroot)
│   ├── Controllers/
│   │      ├── ProductController.cs
│   │      ├── CartController.cs
│   │      ├── CheckoutController.cs
│   │      ├── AdminController.cs
│   │      ├── AccountController.cs
│   │
│   ├── Views/
│   │      ├── Product/ (catalogue, détails)
│   │      ├── Cart/
│   │      ├── Checkout/
│   │      ├── Admin/
│   │      ├── Account/ (Login)
│   │      ├── Shared/ (_Layout.cshtml)
│   │
│   ├── wwwroot/
│         ├── css/
│         ├── js/
│         ├── images/
│         ├── favicon.ico
│
└── README.md
Installation & Exécution
1️⃣ Cloner le repo
git clone https://github.com/ton-username/AkimWigs.git
cd AkimWigs
2️⃣ Modifier appsettings.json

Ajouter ta connexion SQL Server + clés Stripe :
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=AkimWigsDB;Trusted_Connection=True;"
},
"Stripe": {
  "PublishableKey": "pk_test_xxx",
  "SecretKey": "sk_test_xxx"
}
Restaurer & Exécuter
dotnet restore
dotnet build
dotnet run
Accéder au site

Boutique :
👉 https://localhost:7190

Admin :
👉 https://localhost:7190/Admin

Default login :
Email : oseye9
Password : Mamere12
👩🏾‍💼 Auteur

AkimWigs – Boutique en ligne imaginée et développée par El hadji ousmane seye.
MIT License
Copyright (c) 2025 El hadji ousmane seye 
