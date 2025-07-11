using AutoMapper;
using InfertilityTreatment.Business.Helpers;
using InfertilityTreatment.Business.Interfaces;
using InfertilityTreatment.Data.Repositories.Interfaces;
using InfertilityTreatment.Entity.DTOs.Common;
using InfertilityTreatment.Entity.DTOs.Users;
using InfertilityTreatment.Entity.Entities;
using InfertilityTreatment.Entity.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfertilityTreatment.Business.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            // Validate role - only allow Manager and Doctor
            if (request.Role != UserRole.Manager && request.Role != UserRole.Doctor)
            {
                throw new ArgumentException("Role must be either Manager or Doctor");
            }

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(request.Email))
            {
                throw new InvalidOperationException("Email already exists");
            }

            // Check if username already exists
            if (await _userRepository.UsernameExistsAsync(request.Username))
            {
                throw new InvalidOperationException("Username already exists");
            }

            // Create new user entity
            var user = new User
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHelper.HashPassword(request.Password),
                FullName = request.FullName ?? request.Username,
                PhoneNumber = request.PhoneNumber,
                Gender = request.Gender,
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            // Add user to repository
            var createdUser = await _userRepository.AddAsync(user);
            
            // Map to response DTO
            var response = _mapper.Map<UserResponse>(createdUser);
            return response;
        }

        public async Task<string> ChangePasswordAsync(int userId, ChangePasswordDto changePasswordDto)
        {
            var user = await _userRepository.GetByIdAsync(userId)
       ?? throw new KeyNotFoundException("User not found.");
            var isCurrentPasswordCorrect = PasswordHelper.VerifyPassword(changePasswordDto.CurrentPassword, user.PasswordHash);
            if (!isCurrentPasswordCorrect)
                throw new ArgumentException("Current password is incorrect.");

            var isNewPasswordSameAsOld = PasswordHelper.VerifyPassword(changePasswordDto.NewPassword, user.PasswordHash);
            if (isNewPasswordSameAsOld)
                throw new ArgumentException("New password must be different from the current password.");


            var password = PasswordHelper.HashPassword(changePasswordDto.NewPassword);

            var result = await _userRepository.ChangePasswordAsync(userId, password);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";

        }

        public async Task<UserProfileDto> GetProfileAsync(int userId)
        {
            var user = await _userRepository.GetByIdWithProfilesAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            var profileDto = _mapper.Map<UserProfileDto>(user);
            return profileDto;
        }

        public async Task<PaginatedResultDto<UserProfileDto>> GetUsersAsync(UserFilterDto filter)
        {
            var pagedResult = await _userRepository.GetUsers(filter);

            if (pagedResult == null || !pagedResult.Items.Any())
            {
                throw new KeyNotFoundException("No users found.");
            }

            var profileDtos = _mapper.Map<List<UserProfileDto>>(pagedResult.Items);

            return new PaginatedResultDto<UserProfileDto>(
                profileDtos,
                pagedResult.TotalCount,
                pagedResult.PageNumber,
                pagedResult.PageSize
            );
        }

        public async Task<string> UpdateProfileAsync(int userId, UpdateProfileDto updateProfileDto)
        {
            var user = _mapper.Map<User>(updateProfileDto);
            user.Id = userId;

            var result = await _userRepository.UpdateProfile(user);
            if (result == null)
            {
                throw new KeyNotFoundException($"User with ID {userId} not found.");
            }
            return "Profile updated successfully";
        }

    }

}
